using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MMS.DataModels;
using Newtonsoft.Json;

namespace MMS.Backend
{
    class HTTPHandler
    {
        byte[] ServerShutdownSquence = { 0x95, 0xED, 0xE6, 0xAC, 0xB2 };
        Socket HttpServer;
        Thread ServerThread;
        Dictionary<string, Func<string, string>> ConfiguredRoutes;

        public List<NodeModel> RegisterQueue;

        static HTTPHandler _Instance = new HTTPHandler();
        public static HTTPHandler Instance { get => _Instance; }

        public delegate void RegisterRequestReceivedEventHandler(NodeModel node);
        public delegate void HeartbeatReceivedEventHandler(NodeCurrentStatusModel status, string mac);
        public delegate void CommandStatusReceivedEventHandler(CommandLogModel cmdstatus, string mac);

        public event RegisterRequestReceivedEventHandler RegisterRequestReceived;
        public event HeartbeatReceivedEventHandler HeartbeatReceived;
        public event CommandStatusReceivedEventHandler CommandStatusReceived;


        public HTTPHandler()
        {
            RegisterQueue = new List<NodeModel>();
            ConfiguredRoutes = new Dictionary<string, Func<string, string>>();
            ConfiguredRoutes.Add("/node_register/", HandleRegistrationRequest);
            ConfiguredRoutes.Add("/heartbeat/", HandleHeartbeatRequest);
            ConfiguredRoutes.Add("/command_log_api/", HandleCommandStatusRequest);
        }

        public void Start()
        {
            Logging.Debug("Attempting HTTP server socket start");
            try
            {
                HttpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);
                Logging.Debug($"Attempting port binding on HTTP server socket. Port {Globals.Config.HTTPServerPort}");
                HttpServer.Bind(new IPEndPoint(IPAddress.Any, Globals.Config.HTTPServerPort));
                HttpServer.Listen(10);
                Logging.Debug("Starting HTTP handler thread");
                ServerThread = new Thread(new ThreadStart(HandleHTTPRequest));
                ServerThread.Start();
                Logging.Debug("HTTP server socket start successful");
            }
            catch (Exception e)
            {
                Logging.Debug($"Failed to start HTTP server socket. Exception {e.Message}");
            }
        }

        public void Stop()
        {
            Logging.Debug("Attemping HTTP server socket stop");
            try
            {
                if (!ServerThread.IsAlive)
                {
                    Logging.Debug("HTTP server not running");
                    return;
                }
                Socket StopperClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
                StopperClient.SendTimeout = 1000;
                Logging.Debug("Connecting shutdown socket");
                StopperClient.Connect(new IPEndPoint(IPAddress.Loopback, Globals.Config.HTTPServerPort));
                if (StopperClient.Connected)
                {
                    Logging.Debug("Shutdown socket connected. Sending shutdown sequence");
                    StopperClient.Send(ServerShutdownSquence);
                    StopperClient.Close();
                }
                StopperClient.Dispose();
                Thread.Sleep(500);
                if (ServerThread.IsAlive)
                {
                    Logging.Debug("Unexpected. Server thread still running. Killing server thread");
                    try
                    {
                        ServerThread.Abort();
                    }
                    catch (Exception e)
                    {
                        Logging.Debug($"Critical. Server thread could not be aborted. Exception: {e.Message}");
                    }
                }
                HttpServer.Close();
                HttpServer.Dispose();
                Logging.Debug("HTTP server shutdown successful");
            }
            catch (Exception e)
            {
                Logging.Debug($"HTTP server shutdown falied. Exception {e.Message}");
                Logging.Debug($"Attempting to kill server thread");
                try
                {
                    ServerThread.Abort();
                }
                catch (Exception f)
                {
                    Logging.Debug($"Critical. Server thread could not be aborted. Exception: {f.Message}");
                    return;
                }
            }
        }

        private void HandleHTTPRequest()
        {
            while (true)
            {
                try
                {
                    var client = HttpServer.Accept();
                    client.ReceiveTimeout = 3000;
                    byte[] data = new byte[4096];
                    var num = client.Receive(data);
                    if (num == 5)
                    {
                        byte[] seq = new byte[num];
                        Buffer.BlockCopy(data, 0, seq, 0, num);
                        if (seq.SequenceEqual(ServerShutdownSquence))
                        {
                            Logging.Debug("Server shutdown sequence received. Shutting down");
                            client.Close();
                            break;
                        }
                    }
                    var reqString = Encoding.UTF8.GetString(data, 0, num);
                    var respString = "";
                    var reqArr = reqString.Split(new string[] { "\r\n\r\n" }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (reqArr.Length >= 1)
                    {
                        var reqHeaderArr = reqArr[0].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        if (reqHeaderArr.Length >= 1)
                        {
                            var reqRouteArr = reqHeaderArr[0].Split(' ');
                            if (reqRouteArr.Length == 3)
                            {
                                var path = reqRouteArr[1].Trim();
                                if (ConfiguredRoutes.ContainsKey(path))
                                {
                                    if (reqRouteArr[0].Trim().ToUpper() == "POST")
                                    {
                                        if (reqArr.Length == 2)
                                        {
                                            Logging.Debug($"HTTP server handler: post request received at {path}");
                                            respString = ConfiguredRoutes[path].Invoke(reqArr[1]);
                                        }
                                        else
                                        {
                                            Logging.Debug("HTTP server handler: invalid request header");
                                            respString = HandleBadRequest();
                                        }
                                    }
                                    else if (reqRouteArr[0].Trim().ToUpper() == "GET")
                                    {
                                        Logging.Debug($"HTTP server handler: get request received at {path}");
                                        respString = HandleBadRequest();
                                    }
                                    else
                                    {
                                        Logging.Debug("HTTP server handler: unsupported request method");
                                        respString = HandleBadRequest();
                                    }
                                }
                                else
                                {
                                    Logging.Debug($"HTTP server handler: request received at an unconfigured route");
                                    respString = HandleNotFoundRequest();
                                }
                            }
                            else
                            {
                                Logging.Debug("HTTP server handler: invalid request header");
                                respString = HandleBadRequest();
                            }
                        }
                        else
                        {
                            Logging.Debug("HTTP server handler: invalid request header");
                            respString = HandleBadRequest();
                        }
                    }
                    else
                    {
                        Logging.Debug("HTTP server handler: invalid request");
                        respString = HandleBadRequest();
                    }
                    Logging.Debug("HTTP server handler: sending response");
                    client.Send(Encoding.UTF8.GetBytes(respString));
                    client.Close();
                    client.Dispose();
                    Logging.Debug("HTTP server handler: response sent");
                }
                catch (Exception e)
                {
                    Logging.Debug($"HTTP server handler: exception occured {e.Message}. Aborting");
                    break;
                }
            }
        }

        private string HandleRegistrationRequest(string reqBody)
        {
            try
            {
                string response = "";

                var reqmodel = JsonConvert.DeserializeObject<HTTPRegistrationReqModel>(reqBody, new JsonSerializerSettings()
                {
                    Error = (o, e) =>
                    {
                        Logging.Error("Error parsing http registration request. " + e.ErrorContext.Error);
                        e.ErrorContext.Handled = true;
                    }
                });
                if (reqmodel == null)
                {
                    HTTPRegistrationAckModel ackmodel = new HTTPRegistrationAckModel();
                    ackmodel.Status = "REG_INIT";
                    HTTPGeneralRespModel respmodel = new HTTPGeneralRespModel();
                    respmodel.ResponseModel = ackmodel;
                    respmodel.Message = "Incorrect Device Registration Request Format Received";
                    respmodel.IsError = true;
                    string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer";
                    string body = JsonConvert.SerializeObject(respmodel, new JsonSerializerSettings()
                    {
                        Error = (o, e) =>
                        {
                            Logging.Error("Error parsing http registration acknowledgement. " + e.ErrorContext.Error);
                            e.ErrorContext.Handled = true;
                        }
                    });
                    if (body == "") header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
                    return header + "\r\n\r\n" + body;
                }
                try
                {
                    _ = IPAddress.Parse(reqmodel.IP);
                    _ = PhysicalAddress.Parse(reqmodel.MacAddress.Replace(':', '-').ToUpperInvariant());
                }
                catch
                {
                    HTTPRegistrationAckModel ackmodel = new HTTPRegistrationAckModel();
                    ackmodel.Status = "REG_INIT";
                    HTTPGeneralRespModel respmodel = new HTTPGeneralRespModel();
                    respmodel.ResponseModel = ackmodel;
                    respmodel.Message = "Invalid Device Registration Request. Must Provide Valid IP and Mac Addresses.";
                    respmodel.IsError = true;
                    string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer";
                    string body = JsonConvert.SerializeObject(respmodel, new JsonSerializerSettings()
                    {
                        Error = (o, e) =>
                        {
                            Logging.Error("Error parsing http registration acknowledgement. " + e.ErrorContext.Error);
                            e.ErrorContext.Handled = true;
                        }
                    });
                    if (body == "") header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
                    return header + "\r\n\r\n" + body;
                }

                Logging.Info($"Registration request recieved. Mac: {reqmodel.MacAddress}, IP: {reqmodel.IP}");

                NodeModel node = new NodeModel
                {
                    Name = reqmodel.Name ?? "",
                    NodeName = reqmodel.NodeName ?? "",
                    Description = reqmodel.Description ?? "",
                    IP = reqmodel.IP ?? "",
                    MacAddress = reqmodel.MacAddress ?? "",
                    OSType = reqmodel.OSType ?? "",
                    OSName = reqmodel.OSName ?? "",
                    OSArchitecture = reqmodel.OSArch ?? "",
                    Port = reqmodel.Port,
                    SecurePort = reqmodel.SecurePort,
                    RegKey = reqmodel.UniqueRegCode ?? "",
                    PEMFile = reqmodel.PEMFile ?? ""
                };
                node.CurrentStatus.Version = reqmodel.Version ?? "";

                var regq = RegisterQueue.FirstOrDefault(a => a.MacAddress == node.MacAddress);
                if (regq != null)
                {
                    Logging.Info("Processing registartion request");
                    HTTPRegistrationRespModel regrespmodel = new HTTPRegistrationRespModel();
                    regrespmodel.MacAddress = regq.MacAddress;
                    regrespmodel.HeartbeatRate = regq.HeartbeatRate;
                    regrespmodel.Status = "REG_APPROVED";
                    //regrespmodel.AuthToken = Helper.GenerateAuthToken(16);

                    HTTPGeneralRespModel respmodel = new HTTPGeneralRespModel();
                    respmodel.ResponseModel = regrespmodel;
                    respmodel.Message = "Device Registered Successfully.";
                    respmodel.IsError = false;

                    string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer\r\nAccept: application/json\r\nContent-Type: application/json; charset: UTF-8";
                    string body = JsonConvert.SerializeObject(respmodel, new JsonSerializerSettings()
                    {
                        Error = (o, e) =>
                        {
                            Logging.Error("Error parsing http registration response. " + e.ErrorContext.Error);
                            e.ErrorContext.Handled = true;
                        }
                    });
                    if (body == "") header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
                    else
                    {
                        node.HeartbeatRate = regrespmodel.HeartbeatRate;
                        //node.RegKey = regrespmodel.AuthToken;
                        node.FloorID = regq.FloorID;
                        node.ZoneID = regq.ZoneID;
                        node.ExhibitID = regq.ExhibitID;
                        node.IsConfig = true;
                    }
                    RegisterQueue.Remove(regq);

                    Logging.Info("Registration request processed");

                    response = header + "\r\n\r\n" + body;
                }
                else
                {
                    HTTPRegistrationAckModel ackmodel = new HTTPRegistrationAckModel();
                    ackmodel.Status = "REG_INIT";
                    HTTPGeneralRespModel respmodel = new HTTPGeneralRespModel();
                    respmodel.ResponseModel = ackmodel;
                    respmodel.Message = "Device Registration Request Received Successfully";
                    respmodel.IsError = false;
                    string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer";
                    string body = JsonConvert.SerializeObject(respmodel, new JsonSerializerSettings()
                    {
                        Error = (o, e) =>
                        {
                            Logging.Error("Error parsing http registration acknowledgement. " + e.ErrorContext.Error);
                            e.ErrorContext.Handled = true;
                        }
                    });
                    if (body == "") header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
                    response = header + "\r\n\r\n" + body;
                }

                OnRegisterRequestReceived(node);

                return response;
            }
            catch (Exception e)
            {
                Logging.Error("Fatal error occured during registration request processing. " + e.Message);
                string header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
                string body = "";
                string response = header + "\r\n\r\n" + body;
                return response;
            }
        }

        private string HandleHeartbeatRequest(string reqBody)
        {
            var hbmodel = JsonConvert.DeserializeObject<HTTPHeartbeatReqModel>(reqBody, new JsonSerializerSettings()
            {
                Error = (o, e) =>
                {
                    Logging.Error("Error parsing http heartbeat request. " + e.ErrorContext.Error);
                    e.ErrorContext.Handled = true;
                }
            });
            if (hbmodel == null) return "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer\r\n\r\n";

            Logging.Info($"Heartbeat received. Mac: {hbmodel.MacAddress}");
            //var vidlist = JsonConvert.DeserializeObject<string[]>(hbmodel.VideoList, new JsonSerializerSettings()
            //{
            //    Error = (o, e) =>
            //    {
            //        Logging.Error("Error parsing video list from heartbeat. " + e.ErrorContext.Error);
            //        e.ErrorContext.Handled = true;
            //    }
            //});
            NodeCurrentStatusModel status = new NodeCurrentStatusModel
            {
                Temperature = hbmodel.Temperature,
                DiskSpaceUsage = hbmodel.DiskSpaceUsage,
                ProcessorUsage = hbmodel.ProcessorUsage,
                RamUsage = hbmodel.RamUsage,
                Uptime = TimeSpan.FromSeconds(hbmodel.Uptime),
                Version = hbmodel.Version ?? "",
                TotalVideos = hbmodel.TotalVideos,
                //VideoList = vidlist ?? new string[0],
                VideoList = hbmodel.VideoList ?? new string[0],
                VideoName = hbmodel.VideoName ?? "",
                VideoNumber = hbmodel.VideoNumber,
                VideoStatus = hbmodel.VideoStatus ?? "",
                VideoDuration = hbmodel.VideoDuration ?? 0,
                TimeStamp = TimeSpan.FromSeconds(hbmodel.TimeStamp),
                Volume = hbmodel.Volume,
            };

            HTTPHeartbeatAckModel ackmodel = new HTTPHeartbeatAckModel();
            HTTPGeneralRespModel respmodel = new HTTPGeneralRespModel();
            respmodel.ResponseModel = ackmodel;
            respmodel.Message = "Heartbeat Request Received Successfully.";
            respmodel.IsError = false;
            string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer";
            string body = JsonConvert.SerializeObject(respmodel, new JsonSerializerSettings()
            {
                Error = (o, e) =>
                {
                    Logging.Error("Error parsing http heartbeat acknowledgement. " + e.ErrorContext.Error);
                    e.ErrorContext.Handled = true;
                }
            });
            if (body == "") header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
            string response = header + "\r\n\r\n" + body;

            OnHeartbeatReceived(status, hbmodel.MacAddress);

            return response;
        }

        private string HandleCommandStatusRequest(string reqBody)
        {
            var csmodel = JsonConvert.DeserializeObject<HTTPCommandStatusReqModel>(reqBody, new JsonSerializerSettings()
            {
                Error = (o, e) =>
                {
                    Logging.Error("Error parsing http command status request. " + e.ErrorContext.Error);
                    e.ErrorContext.Handled = true;
                }
            });
            if (csmodel == null) return "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer\r\n\r\n";

            Logging.Info($"Command status received. ID: {csmodel.ID}");
            CommandLogModel logmodel = new CommandLogModel
            {
                CommandID = csmodel.ID,
                Status = csmodel.Status,
                Message = csmodel.Message,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            HTTPCommandStatusAckModel ackmodel = new HTTPCommandStatusAckModel();
            HTTPGeneralRespModel respmodel = new HTTPGeneralRespModel
            {
                ResponseModel = ackmodel,
                Message = "Command status received successfully.",
                IsError = false
            };
            string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer";
            string body = JsonConvert.SerializeObject(respmodel, new JsonSerializerSettings()
            {
                Error = (o, e) =>
                {
                    Logging.Error("Error parsing http command status acknowledgement. " + e.ErrorContext.Error);
                    e.ErrorContext.Handled = true;
                }
            });
            if (body == "") header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
            string response = header + "\r\n\r\n" + body;

            OnCommandStatusReceived(logmodel, csmodel.MacAddress);

            return response;
        }

        private string HandleBadRequest()
        {
            string header = "HTTP/1.1 400 Bad Request\r\nServer: MMSHTTPServer\r\n";
            string body = "";
            string response = header + body;
            return response;
        }

        private string HandleNotFoundRequest()
        {
            string header = "HTTP/1.1 404 Not Found\r\nServer: MMSHTTPServer\r\n";
            string body = "";
            string response = header + body;
            return response;
        }


        private void OnRegisterRequestReceived(NodeModel node)
        {
            RegisterRequestReceived?.Invoke(node);
        }

        private void OnHeartbeatReceived(NodeCurrentStatusModel status, string mac)
        {
            HeartbeatReceived?.Invoke(status, mac);
        }

        private void OnCommandStatusReceived(CommandLogModel cmdstatus, string mac)
        {
            CommandStatusReceived?.Invoke(cmdstatus, mac);
        }
    }
}
