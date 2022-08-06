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
        //public static ObservableCollection<NodeModel> AllDevices { get; set; }

        //public static event NotifyCollectionChangedEventHandler AllDeviceListChanged;

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
            //AllDevices = new ObservableCollection<NodeModel>();

            //AllDevices.CollectionChanged += AllDevicesCollectionChanged;
            HTTPHandler.Instance.RegisterRequestReceived += HTTPRegisterRequestReceived;
            HTTPHandler.Instance.HeartbeatReceived += HTTPHeartbeatReceived;
            HTTPHandler.Instance.CommandStatusReceived += HTTPCommandStatusReceived;

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

            //FillReqData();

            var read_floors = DBInterface.ReadFloorData();
            foreach (var fl in read_floors) DataHub.Floors.Add(fl);

            var read_commands = DBInterface.ReadCommandData();
            foreach (var cmd in read_commands) DataHub.Commands.Add(cmd);

            var read_commandlogs = DBInterface.ReadCommandLogData();
            foreach (var cmdlog in read_commandlogs) DataHub.CommandLogs.Add(cmdlog);

            var read_devices = DBInterface.ReadNodeData();
            var read_status = DBInterface.ReadNodeStatusData();
            foreach (var dev in read_devices)
            {
                dev.IsOnline = false;
                dev.CurrentStatus = read_status.FirstOrDefault(a => a.NodeID == dev.ID) ?? new NodeCurrentStatusModel();
                DataHub.Nodes.Add(dev);
            }

            DeviceOnlineTimer.Change(0, 30000);
            HTTPHandler.Instance.Start();
            return true;
        }

        public static void Destroy()
        {
            HTTPHandler.Instance.Stop();
            DeviceOnlineTimer.Dispose();
            //AllDevices.Clear();
            Logging.Release();
        }

        public static void FillReqData()
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

        public static void RegisterDevice(NodeModel device)
        {
            HTTPHandler.Instance.RegisterQueue.Add(device);
        }

        public static void SendCommand(CommandModel command, NodeModel node, bool secure)
        {
            var cmdbytes = Encoding.UTF8.GetBytes(command.Command);
            if (secure)
            {
                var cmdbytesenc = TCPHandler.Instance.EncryptFromPublicKey(cmdbytes, node.PEMFile);
                TCPHandler.Instance.Send(cmdbytesenc, node.IP, node.SecurePort);
            }
            else TCPHandler.Instance.Send(cmdbytes, node.IP, node.Port);
        }

        //private static void AllDevicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            foreach (NodeModel a in e.NewItems)
        //            {
        //                OnlineCheckList.Add(AllDevices.IndexOf(a), false);
        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            foreach (NodeModel a in e.OldItems)
        //            {
        //                OnlineCheckList.Remove(AllDevices.IndexOf(a));
        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Replace:
        //            break;
        //        case NotifyCollectionChangedAction.Reset:
        //            OnlineCheckList.Clear();
        //            break;
        //    }
        //    OnAllDeviceListChanged(sender, e);
        //}

        private static void HTTPRegisterRequestReceived(NodeModel node)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dev = DataHub.Nodes.FirstOrDefault(a => a.MacAddress == node.MacAddress);
                if (dev == null)
                {
                    var temp = new List<NodeModel>() { node };
                    DBInterface.WriteNodeData(ref temp);
                    temp[0].CurrentStatus.NodeID = temp[0].ID;
                    DBInterface.WriteNodeStatusData(ref temp);
                    DataHub.Nodes.Add(temp[0]);
                }
                else
                {
                    var index = DataHub.Nodes.IndexOf(dev);

                    if (DataHub.Nodes[index].IsConfig)
                    {
                        Logging.Error($"Registration request received for already configured device {DataHub.Nodes[index].MacAddress}");
                        return;
                    }

                    DataHub.Nodes[index].Name = node.Name;
                    DataHub.Nodes[index].NodeName = node.NodeName;
                    DataHub.Nodes[index].Description = node.Description;
                    DataHub.Nodes[index].IP = node.IP;
                    DataHub.Nodes[index].Port = node.Port;
                    DataHub.Nodes[index].SecurePort = node.SecurePort;
                    DataHub.Nodes[index].IsConfig = node.IsConfig;
                    DataHub.Nodes[index].RegKey = node.RegKey;
                    DataHub.Nodes[index].OSType = node.OSType;
                    DataHub.Nodes[index].OSName = node.OSName;
                    DataHub.Nodes[index].OSArchitecture = node.OSArchitecture;
                    DataHub.Nodes[index].PEMFile = node.PEMFile;
                    DataHub.Nodes[index].CurrentStatus.Version = node.CurrentStatus.Version;
                    if (node.IsConfig)
                    {
                        DataHub.Nodes[index].HeartbeatRate = node.HeartbeatRate;
                        DataHub.Nodes[index].FloorID = node.FloorID;
                        DataHub.Nodes[index].ZoneID = node.ZoneID;
                        DataHub.Nodes[index].ExhibitID = node.ExhibitID;
                    }
                    //AllDevices[index] = AllDevices[index];

                    var obj = DataHub.Nodes[index];
                    var temp = new List<NodeModel>() { obj };
                    DBInterface.WriteNodeData(ref temp, true);
                    DBInterface.WriteNodeStatusData(ref temp, true);
                }
            });
        }

        private static void HTTPHeartbeatReceived(NodeCurrentStatusModel status, string mac)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dev = DataHub.Nodes.FirstOrDefault(a => a.MacAddress == mac);
                if (dev != null)
                {
                    var index = DataHub.Nodes.IndexOf(dev);
                    OnlineCheckList[index] = true;
                    DataHub.Nodes[index].IsOnline = true;
                    DataHub.Nodes[index].CurrentStatus.Temperature = status.Temperature;
                    DataHub.Nodes[index].CurrentStatus.Uptime = status.Uptime;
                    DataHub.Nodes[index].CurrentStatus.ProcessorUsage = status.ProcessorUsage;
                    DataHub.Nodes[index].CurrentStatus.DiskSpaceUsage = status.DiskSpaceUsage;
                    DataHub.Nodes[index].CurrentStatus.RamUsage = status.RamUsage;
                    DataHub.Nodes[index].CurrentStatus.VideoName = status.VideoName;
                    DataHub.Nodes[index].CurrentStatus.VideoNumber = status.VideoNumber;
                    DataHub.Nodes[index].CurrentStatus.VideoStatus = status.VideoStatus;
                    DataHub.Nodes[index].CurrentStatus.VideoDuration = status.VideoDuration;
                    DataHub.Nodes[index].CurrentStatus.VideoList = status.VideoList;
                    DataHub.Nodes[index].CurrentStatus.TotalVideos = status.TotalVideos;
                    DataHub.Nodes[index].CurrentStatus.TimeStamp = status.TimeStamp;
                    DataHub.Nodes[index].CurrentStatus.Volume = status.Volume;
                    DataHub.Nodes[index].CurrentStatus.Version = status.Version;
                    //AllDevices[index] = AllDevices[index];

                    var obj = DataHub.Nodes[index];
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

        private static void HTTPCommandStatusReceived(CommandLogModel cmdstatus, string mac)
        {
            var dev = DataHub.Nodes.FirstOrDefault(a => a.MacAddress == mac);
            if (dev != null)
            {
                cmdstatus.NodeID = dev.ID;
                var temp = new List<CommandLogModel>() { cmdstatus };
                DBInterface.WriteCommandLogData(ref temp);
                DataHub.CommandLogs.Add(temp[0]);
            }
        }

        private static void DeviceOnlineCheckExpire(object sender)
        {
            for (int i = 0; i < DataHub.Nodes.Count; i++)
            {
                if (!OnlineCheckList.Keys.Contains(i)) return;
                if (!OnlineCheckList[i])
                {
                    DataHub.Nodes[i].IsOnline = false;
                    //AllDevices[i] = AllDevices[i];
                }
                OnlineCheckList[i] = false;
            }
        }

        //private static void OnAllDeviceListChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    AllDeviceListChanged?.Invoke(sender, e);
        //}
    }
}
