using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMS.DataModels;
using MMS.Backend.DatabaseIO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;

namespace MMS.Backend
{
    class Globals
    {
        public static ConfigurationModel Config { get; set; }
        public static IDatabaseIO DBInterface { get; set; }
        public static ObservableCollection<NodeModel> AllDevices { get; set; }

        public static event NotifyCollectionChangedEventHandler AllDeviceListChanged;

        private static Timer DeviceOnlineTimer;
        private static Dictionary<int, bool> OnlineCheckList;

        public static bool Initialize()
        {
            Logging.Initialize();
            Logging.EnableDebugLogging();
            Config = Configuration.Parse();
            if (Config == null)
            {
                Logging.Error("Error loading configuration");
                return false;
            }
            DeviceOnlineTimer = new Timer(DeviceOnlineCheckExpire);
            OnlineCheckList = new Dictionary<int, bool>();
            DBInterface = new MsSQLDatabaseIO();
            AllDevices = new ObservableCollection<NodeModel>();

            AllDevices.CollectionChanged += AllDevicesCollectionChanged;
            HTTPHandler.Instance.RegisterRequestReceived += HTTPRegisterRequestReceived;
            HTTPHandler.Instance.HeartbeatReceived += HTTPHeartbeatReceived;

            var result = DBInterface.CheckDatabaseValidity();
            if (result == DatabaseCheckResult.NotFound)
            {
                if (DBInterface.CreateDatabase())
                {
                    Thread.Sleep(5000);
                    if (!DBInterface.CreateTables())
                    {
                        Logging.Error("Could not create database tables");
                        return false;
                    }
                }
                else
                {
                    Logging.Error("Could not create database");
                    return false;
                }
            }
            else if (result == DatabaseCheckResult.TableCorrupt)
            {
                Logging.Error("Existing database found with corrupted tables");
                return false;
            }
            else if (result == DatabaseCheckResult.Exception)
            {
                Logging.Error("Unexpected exception occured while setting up database");
                return false;
            }

            FillReqData(false);

            var read_devices = DBInterface.ReadNodeData();
            var read_status = DBInterface.ReadNodeStatusData();
            foreach (var dev in read_devices)
            {
                dev.IsOnline = false;
                dev.CurrentStatus = read_status.FirstOrDefault(a => a.NodeID == dev.ID) ?? new NodeCurrentStatusModel();
                AllDevices.Add(dev);
            }

            DeviceOnlineTimer.Change(0, 30000);
            HTTPHandler.Instance.Start();
            return true;
        }

        public static void Destroy()
        {
            HTTPHandler.Instance.Stop();
            DeviceOnlineTimer.Dispose();
            AllDevices.Clear();
            Logging.Release();
        }

        public static void FillReqData(bool fill)
        {
            if (fill)
            {
                FloorModel fmode = new FloorModel
                {
                    IsActive = true,
                    Name = "Test Floor"
                };
                ZoneModel zmode = new ZoneModel
                {
                    IsActive = true,
                    Name = "Test Zone"
                };
                ExhibitModel exmode = new ExhibitModel
                {
                    IsActive = true,
                    IsExhibitShow = true,
                    Name = "Test Exhibit"
                };
                List<FloorModel> fl = new List<FloorModel> { fmode };
                List<ZoneModel> zl = new List<ZoneModel> { zmode };
                List<ExhibitModel> exl = new List<ExhibitModel> { exmode };
                DBInterface.WriteFloorData(ref fl);
                zl[0].FloorID = fl[0].ID;
                DBInterface.WriteZoneData(ref zl);
                exl[0].ZoneID = zl[0].ID;
                DBInterface.WriteExhibitData(ref exl);
            }
        }

        public static void RegisterDevice(NodeModel device)
        {
            HTTPHandler.Instance.RegisterQueue.Add(device);
        }

        private static void AllDevicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (NodeModel a in e.NewItems)
                    {
                        OnlineCheckList.Add(AllDevices.IndexOf(a), false);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (NodeModel a in e.OldItems)
                    {
                        OnlineCheckList.Remove(AllDevices.IndexOf(a));
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnlineCheckList.Clear();
                    break;
            }
            OnAllDeviceListChanged(sender, e);
        }

        private static void HTTPRegisterRequestReceived(NodeModel node)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dev = AllDevices.FirstOrDefault(a => a.MacAddress == node.MacAddress);
                if (dev == null)
                {
                    var temp = new List<NodeModel>() { node };
                    DBInterface.WriteNodeData(ref temp);
                    temp[0].CurrentStatus.NodeID = temp[0].ID;
                    DBInterface.WriteNodeStatusData(ref temp);
                    AllDevices.Add(temp[0]);
                }
                else
                {

                    var index = AllDevices.IndexOf(dev);

                    if (AllDevices[index].IsConfig)
                    {
                        Logging.Error($"Registration request received for already configured device {AllDevices[index].MacAddress}");
                        return;
                    }

                    AllDevices[index].Name = node.Name;
                    AllDevices[index].NodeName = node.NodeName;
                    AllDevices[index].Description = node.Description;
                    AllDevices[index].IP = node.IP;
                    AllDevices[index].Port = node.Port;
                    AllDevices[index].IsConfig = node.IsConfig;
                    AllDevices[index].RegKey = node.RegKey;
                    AllDevices[index].OSType = node.OSType;
                    AllDevices[index].OSName = node.OSName;
                    AllDevices[index].OSArchitecture = node.OSArchitecture;
                    AllDevices[index].CurrentStatus.Version = node.CurrentStatus.Version;
                    if (node.IsConfig)
                    {
                        AllDevices[index].HeartbeatRate = node.HeartbeatRate;
                        AllDevices[index].FloorID = node.FloorID;
                        AllDevices[index].ZoneID = node.ZoneID;
                        AllDevices[index].ExhibitID = node.ExhibitID;
                    }
                    AllDevices[index] = AllDevices[index];

                    var obj = AllDevices[index];
                    var temp = new List<NodeModel>() { obj };
                    DBInterface.WriteNodeData(ref temp, true);
                    DBInterface.WriteNodeStatusData(ref temp, true);
                }
            });
        }

        private static void HTTPHeartbeatReceived(NodeCurrentStatusModel status, IPEndPoint remoteEndpoint)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dev = AllDevices.FirstOrDefault(a => a.IP == remoteEndpoint.Address.ToString() && a.Port == remoteEndpoint.Port);
                if (dev != null)
                {
                    var index = AllDevices.IndexOf(dev);
                    OnlineCheckList[index] = true;
                    AllDevices[index].IsOnline = true;
                    AllDevices[index].CurrentStatus.Temperature = status.Temperature;
                    AllDevices[index].CurrentStatus.Uptime = status.Uptime;
                    AllDevices[index].CurrentStatus.ProcessorUsage = status.ProcessorUsage;
                    AllDevices[index].CurrentStatus.DiskSpaceUsage = status.DiskSpaceUsage;
                    AllDevices[index].CurrentStatus.RamUsage = status.RamUsage;
                    AllDevices[index].CurrentStatus.VideoName = status.VideoName;
                    AllDevices[index].CurrentStatus.VideoNumber = status.VideoNumber;
                    AllDevices[index].CurrentStatus.VideoStatus = status.VideoStatus;
                    AllDevices[index].CurrentStatus.VideoDuration = status.VideoDuration;
                    AllDevices[index].CurrentStatus.VideoList = status.VideoList;
                    AllDevices[index].CurrentStatus.TotalVideos = status.TotalVideos;
                    AllDevices[index].CurrentStatus.TimeStamp = status.TimeStamp;
                    AllDevices[index].CurrentStatus.Volume = status.Volume;
                    AllDevices[index].CurrentStatus.Version = status.Version;
                    AllDevices[index] = AllDevices[index];

                    var obj = AllDevices[index];
                    var temp = new List<NodeModel>() { obj };
                    DBInterface.WriteNodeStatusData(ref temp, true);

                    var logobj = new NodeLogModel
                    {
                        NodeID = obj.ID,
                        Uptime = obj.CurrentStatus.Uptime,
                        Temperature = obj.CurrentStatus.Temperature,
                        ProcessorUsage = obj.CurrentStatus.ProcessorUsage,
                        DiskSpaceUsage = obj.CurrentStatus.DiskSpaceUsage,
                        RamUsage = obj.CurrentStatus.RamUsage,
                        Version = obj.CurrentStatus.Version
                    };
                    var templ = new List<NodeLogModel>() { logobj };
                    DBInterface.WriteNodeLogData(ref templ);
                }
            });
        }

        private static void DeviceOnlineCheckExpire(object sender)
        {
            for (int i = 0; i < AllDevices.Count; i++)
            {
                if (!OnlineCheckList.Keys.Contains(i)) return;
                if (!OnlineCheckList[i])
                {
                    AllDevices[i].IsOnline = false;
                    AllDevices[i] = AllDevices[i];
                }
                OnlineCheckList[i] = false;
            }
        }

        private static void OnAllDeviceListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            AllDeviceListChanged?.Invoke(sender, e);
        }
    }
}
