using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
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

        IPEndPoint clientEndpoint; //Temporary workaround

        public List<NodeModel> RegisterQueue;
        
        static HTTPHandler _Instance = new HTTPHandler();
        public static HTTPHandler Instance { get => _Instance; }

        public delegate void RegisterRequestReceivedEventHandler(NodeModel node);
        public delegate void HeartbeatReceivedEventHandler(NodeCurrentStatusModel status, IPEndPoint remoteEndpoint); //Temporary workaround for id

        public event RegisterRequestReceivedEventHandler RegisterRequestReceived;
        public event HeartbeatReceivedEventHandler HeartbeatReceived;


        public HTTPHandler()
        {
            RegisterQueue = new List<NodeModel>();
            ConfiguredRoutes = new Dictionary<string, Func<string, string>>();
            ConfiguredRoutes.Add("/node_register/", HandleRegistrationRequest);
            ConfiguredRoutes.Add("/heartbeat/", HandleHeartbeatRequest);
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
            catch(Exception e)
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
                    catch(Exception e)
                    {
                        Logging.Debug($"Critical. Server thread could not be aborted. Exception: {e.Message}");
                    }
                }
                HttpServer.Close();
                HttpServer.Dispose();
                Logging.Debug("HTTP server shutdown successful");
            }
            catch(Exception e)
            {
                Logging.Debug($"HTTP server shutdown falied. Exception {e.Message}");
                Logging.Debug($"Attempting to kill server thread");
                try
                {
                    ServerThread.Abort();
                }
                catch(Exception f)
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
                                            clientEndpoint = (IPEndPoint)client.RemoteEndPoint; //Temporary workaround
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
                                        Logging.Debug("HTTP server handler: invalid request header");
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
                        Logging.Debug("HTTP server handler: invalid request header");
                        respString = HandleBadRequest();
                    }
                    Logging.Debug("HTTP server handler: sending response");
                    client.Send(Encoding.UTF8.GetBytes(respString));
                    client.Close();
                    client.Dispose();
                    Logging.Debug("HTTP server handler: response sent");
                }
                catch(Exception e)
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

                var reqmodel = JsonConvert.DeserializeObject<HTTPRequestModel>(reqBody, new JsonSerializerSettings()
                {
                    Error = (o, e) =>
                    {
                        Logging.Error("Error parsing http registration request. " + e.ErrorContext.Error);
                    }
                });
                if (reqmodel == null) return "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";

                Logging.Info($"Registration request recieved. Mac: {reqmodel.MacAddress}, IP: {reqmodel.IP}");

                NodeModel node = new NodeModel
                {
                    Name = reqmodel.Name,
                    NodeName = reqmodel.NodeName,
                    Description = reqmodel.Description,
                    IP = reqmodel.IP,
                    MacAddress = reqmodel.MacAddress,
                    OSType = reqmodel.OSType,
                    OSName = reqmodel.OSName,
                    OSArchitecture = reqmodel.OSArch,
                    Port = reqmodel.Port,
                    RegKey = reqmodel.UniqueRegCode,
                };
                node.CurrentStatus.Version = reqmodel.Version;

                var regq = RegisterQueue.FirstOrDefault(a => a.MacAddress == reqmodel.MacAddress);
                if (regq != null)
                {
                    Logging.Info("Processing registartion request");
                    HTTPResponseModel respmodel = new HTTPResponseModel();
                    respmodel.MacAddress = reqmodel.MacAddress;
                    respmodel.HeartbeatRate = regq.HeartbeatRate;
                    respmodel.Status = "REG_APPROVED";
                    respmodel.AuthToken = Helper.GenerateAuthToken(16);
                    Dictionary<string, HTTPResponseModel> reply = new Dictionary<string, HTTPResponseModel>();
                    reply.Add("success", respmodel);

                    string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer\r\nAccept: application/json\r\nContent-Type: application/json; charset: UTF-8\r\n\r\n";
                    string body = JsonConvert.SerializeObject(reply, new JsonSerializerSettings()
                    {
                        Error = (o, e) =>
                        {
                            Logging.Error("Error parsing http registration response. " + e.ErrorContext.Error);
                        }
                    });
                    if (body == "") header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
                    else
                    {
                        node.HeartbeatRate = respmodel.HeartbeatRate;
                        node.RegKey = respmodel.AuthToken;
                        node.FloorID = regq.FloorID;
                        node.ZoneID = regq.ZoneID;
                        node.ExhibitID = regq.ExhibitID;
                        node.IsConfig = true;
                    }
                    RegisterQueue.Remove(regq);

                    Logging.Info("Registration request processed");

                    response = header + body;
                }
                else
                {
                    string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer";
                    string body = "";
                    response = header + body;
                }

                OnRegisterRequestReceived(node);

                return response;
            }
            catch(Exception e)
            {
                string header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
                string body = "";
                string response = header + body;
                return response;
            }
        }

        private string HandleHeartbeatRequest(string reqBody)
        {
            var hbmodel = JsonConvert.DeserializeObject<HTTPHeartbeatModel>(reqBody, new JsonSerializerSettings()
            {
                Error = (o, e) =>
                {
                    Logging.Error("Error parsing http heartbeat request. " + e.ErrorContext.Error);
                }
            });
            if (hbmodel == null) return "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";

            Logging.Info($"Heartbeat received. IP: {clientEndpoint.Address}");
            NodeCurrentStatusModel status = new NodeCurrentStatusModel
            {
                Temperature = hbmodel.Temperature,
                DiskSpaceUsage = hbmodel.DiskSpaceUsage,
                ProcessorUsage = hbmodel.ProcessorUsage,
                RamUsage = hbmodel.RamUsage,
                Uptime = TimeSpan.FromSeconds(hbmodel.Uptime),
                Version = hbmodel.Version,
                TotalVideos = hbmodel.TotalVideos,
                VideoList = hbmodel.VideoList,
                VideoName = hbmodel.VideoName,
                VideoNumber = hbmodel.VideoNumber,
                VideoStatus = hbmodel.VideoStatus,
                VideoDuration = hbmodel.VideoDuration ?? 0,
                TimeStamp = TimeSpan.FromSeconds(hbmodel.TimeStamp),
                Volume = hbmodel.Volume
            };
            
            string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer\r\n";
            string body = "";
            string response = header + body;

            OnHeartbeatReceived(status);

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

        private void OnHeartbeatReceived(NodeCurrentStatusModel status)
        {
            HeartbeatReceived?.Invoke(status, clientEndpoint);
        }
    }
}
