using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using MMS.Backend.DatabaseIO;
using MMS.DataModels;

namespace MMS.Backend
{
    class Globals
    {
        public delegate void PushNotifyRequestedEventHandler(string title, string message, string type);
        public static event PushNotifyRequestedEventHandler PushNotifyRequested;

        public static ConfigurationModel Config { get; set; }
        public static IDatabaseIO DBInterface { get; set; }

        private static Timer DeviceOnlineTimer;
        private static Dictionary<int, bool> OnlineCheckList;

        public static Dictionary<string, long> CommandHistory = new Dictionary<string, long>();

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
            DBInterface = new SQLiteDatabaseIO();

            SetupHttpFilesFolder();

            HTTPHandler.Instance.RegisterRequestReceived += HTTPRegisterRequestReceived;
            HTTPHandler.Instance.HeartbeatReceived += HTTPHeartbeatReceived;
            HTTPHandler.Instance.CommandStatusReceived += HTTPCommandStatusReceived;

            var result = DBInterface.CheckDatabaseValidity();
            if (result == DatabaseCheckResult.NotFound)
            {
                if (DBInterface.CreateDatabase())
                {
                    Thread.Sleep(3000);
                    if (!DBInterface.CreateTables())
                    {
                        Logging.Error("Could not create database tables");
                        return false;
                    }
                    Thread.Sleep(1000);
                    FillReqData();
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

            var read_floors = DBInterface.ReadFloorData();
            foreach (var fl in read_floors) DataHub.Floors.Add(fl);

            var read_commands = DBInterface.ReadCommandData();
            foreach (var cmd in read_commands) DataHub.Commands.Add(cmd);

            var read_commandlogs = DBInterface.ReadCommandLogData();
            foreach (var cmdlog in read_commandlogs) DataHub.CommandLogs.Add(cmdlog);

            var read_devices = DBInterface.ReadNodeData();
            //var read_status = DBInterface.ReadNodeStatusData();
            var read_files = DBInterface.ReadNodeFileData();
            foreach (var dev in read_devices)
            {
                dev.IsOnline = false;
                //dev.CurrentStatus = read_status.FirstOrDefault(a => a.NodeID == dev.ID) ?? new NodeCurrentStatusModel();
                dev.CurrentStatus = new NodeCurrentStatusModel();
                var dev_files = read_files.Where(a => a.NodeID == dev.ID);
                foreach (var dev_file in dev_files) dev.Files.Add(dev_file);
                DataHub.Nodes.Add(dev);
            }

            CommandHistory.Clear();
            DeviceOnlineTimer.Change(0, 30000);
            HTTPHandler.Instance.Start();
            return true;
        }

        public static void Destroy()
        {
            HTTPHandler.Instance.Stop();
            DeviceOnlineTimer.Dispose();
            Logging.Release();
        }

        public static void SetupHttpFilesFolder()
        {
            if (!Directory.Exists(@"files"))
            {
                Directory.CreateDirectory(@"files");
                Directory.CreateDirectory(@"files\media\content");
                Directory.CreateDirectory(@"files\media\software");
            }
            else
            {
                if (!Directory.Exists(@"files\media\content")) Directory.CreateDirectory(@"files\media\content");
                if (!Directory.Exists(@"files\media\software")) Directory.CreateDirectory(@"files\media\software");
            }
        }

        public static void FillReqData()
        {
            List<CommandModel> c1 = new List<CommandModel>
            {
                new CommandModel
                {
                    CommandName = "Halt",
                    Command = "halt",
                    CommandNumber = (int)CommandNumber.Halt,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Run",
                    Command = "run",
                    CommandNumber = (int)CommandNumber.Play,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Reset",
                    Command = "restart",
                    CommandNumber = (int)CommandNumber.Reset,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Reboot",
                    Command = "reboot",
                    CommandNumber = (int)CommandNumber.Restart,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Next",
                    Command = "nextVideo",
                    CommandNumber = (int)CommandNumber.Next,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Previous",
                    Command = "previousVideo",
                    CommandNumber = (int)CommandNumber.Previous,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Goto Time",
                    Command = "gotoTime",
                    CommandNumber = (int)CommandNumber.GotoTime,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Volume",
                    Command = "VOLUME",
                    CommandNumber = (int)CommandNumber.VolumeValue,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Play By Name",
                    Command = "playByName",
                    CommandNumber = (int)CommandNumber.PlayByName,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "ShutDown",
                    Command = "shutdown",
                    CommandNumber = (int)CommandNumber.ShutDown,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Power On",
                    Command = "TurnOn",
                    CommandNumber = (int)CommandNumber.PowerOn,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Add Content",
                    Command = "ADD_CONTENT",
                    CommandNumber = (int)CommandNumber.AddContent,
                    IsEnabled = true
                },
                new CommandModel
                {
                    CommandName = "Software Update",
                    Command = "SOFTWARE_UPDATE",
                    CommandNumber = (int)CommandNumber.Update,
                    IsEnabled = true
                }
            };
            List<FloorModel> f1 = new List<FloorModel>
            {
                new FloorModel
                {
                    Name = "Floor 1",
                    IsActive = true,
                    Description = "Floor 1"
                },
                new FloorModel
                {
                    Name = "Floor 2",
                    IsActive = true,
                    Description = "Floor 2"
                },
                new FloorModel
                {
                    Name = "Floor 3",
                    IsActive = true,
                    Description = "Floor 3"
                }
            };
            DBInterface.WriteCommandData(ref c1);
            DBInterface.WriteFloorData(ref f1);
        }

        public static void RegisterDevice(NodeModel device)
        {
            HTTPHandler.Instance.RegisterQueue.Add(device);
        }

        public static void SendCommand(CommandModel command, NodeModel node, bool secure)
        {
            if (command == null)
            {
                Logging.Error("Send Command Failed. Command Not Found");
                return;
            }
            if (node == null)
            {
                Logging.Error("Send Command Failed. Node Not Found");
                return;
            }
            var cmdbytes = Encoding.UTF8.GetBytes(command.GetFinalizedCommand());
            CommandHistory.Add(command.SessionID, command.ID);
            if (secure)
            {
                var cmdbytesenc = TCPHandler.Instance.EncryptFromPublicKey(cmdbytes, node.PEMFile);
                TCPHandler.Instance.Send(cmdbytesenc, node.IP, node.SecurePort);
            }
            else TCPHandler.Instance.Send(cmdbytes, node.IP, node.Port);
            var cmdlog = new CommandLogModel
            {
                CommandID = command.ID,
                NodeID = node.ID,
                CommandSessionID = command.SessionID,
                Status = "SUCCESS",
                Message = "TCP request sent successfully",
                UpdatedBy = "MMS",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var temp = new List<CommandLogModel>() { cmdlog };
            DBInterface.WriteCommandLogData(ref temp);
            DataHub.CommandLogs.Add(temp[0]);
            Logging.Info("Send Command Successful");
        }

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
                        if (node.IsConfig) return;
                        RegisterDevice(node);
                        //Logging.Error($"Registration request received for already configured device {DataHub.Nodes[index].MacAddress}");
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
                        DataHub.Nodes[index].Category = node.Category;
                        DataHub.Nodes[index].FloorID = node.FloorID;
                        DataHub.Nodes[index].ZoneID = node.ZoneID;
                        DataHub.Nodes[index].ExhibitID = node.ExhibitID;
                        DataHub.Nodes[index].IsOnline = true;
                        OnlineCheckList[index] = true;
                        OnPushNotifyRequested("Info", "Device Registered Successfully", "info");
                    }

                    var obj = DataHub.Nodes[index];
                    var temp = new List<NodeModel>() { obj };
                    DBInterface.WriteNodeData(ref temp, true);
                    DBInterface.WriteNodeStatusData(ref temp, true);
                    DataHub.Nodes[index].UpdatedAt = temp[0].UpdatedAt;
                    DataHub.Nodes[index].CurrentStatus.UpdatedAt = temp[0].CurrentStatus.UpdatedAt;
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

                    var obj = DataHub.Nodes[index];
                    var temp = new List<NodeModel>() { obj };
                    DBInterface.WriteNodeStatusData(ref temp, true);
                    DataHub.Nodes[index].CurrentStatus.UpdatedAt = temp[0].CurrentStatus.UpdatedAt;

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
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dev = DataHub.Nodes.FirstOrDefault(a => a.MacAddress == mac);
                if (dev != null)
                {
                    cmdstatus.NodeID = dev.ID;
                    cmdstatus.CommandID = CommandHistory[cmdstatus.CommandSessionID];
                    var temp = new List<CommandLogModel>() { cmdstatus };
                    DBInterface.WriteCommandLogData(ref temp);
                    DataHub.CommandLogs.Add(temp[0]);
                }
            });
        }

        private static void DeviceOnlineCheckExpire(object sender)
        {
            for (int i = 0; i < DataHub.Nodes.Count; i++)
            {
                if (!OnlineCheckList.Keys.Contains(i)) return;
                if (!OnlineCheckList[i])
                {
                    DataHub.Nodes[i].IsOnline = false;
                }
                OnlineCheckList[i] = false;
            }
        }

        private static void OnPushNotifyRequested(string title, string message, string type)
        {
            PushNotifyRequested?.Invoke(title, message, type);
        }
    }

    public enum CommandNumber
    {
        Play,
        Pause,
        GotoTime,
        PlayByName,
        Mute,
        VolumeValue,
        Previous,
        Next,
        Reset,
        Halt,
        Restart,
        ShutDown,
        PowerOn,
        AddContent,
        Update
    }
}
