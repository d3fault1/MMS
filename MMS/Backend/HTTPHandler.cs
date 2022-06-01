using System;
using System.Collections.Generic;
using System.Net;
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

        public void Start()
        {
            ConfiguredRoutes = new Dictionary<string, Func<string, string>>();
            ConfiguredRoutes.Add("/node_register/", HandleRegistrationRequest);
            ConfiguredRoutes.Add("/heartbeat/", HandleHeartbeatRequest);

            HttpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);
            HttpServer.Bind(new IPEndPoint(IPAddress.Any, Globals.Config.HTTPServerPort));
            HttpServer.Listen(10);
            ServerThread = new Thread(new ThreadStart(HandleHTTPRequest));
            ServerThread.Start();
        }

        public void Stop()
        {
            Socket StopperClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
            StopperClient.SendTimeout = 1000;
            StopperClient.Connect(new IPEndPoint(IPAddress.Any, 2628));
            if (StopperClient.Connected)
            {
                StopperClient.Send(ServerShutdownSquence);
                StopperClient.Close();
            }
            StopperClient.Dispose();
            HttpServer.Close();
            HttpServer.Dispose();
        }

        private void HandleHTTPRequest()
        {
            while (true)
            {
                var client = HttpServer.Accept();
                if (client.Available == 0) continue;
                byte[] data = new byte[4096];
                var num = client.Receive(data);
                if (new byte[] { data[0], data[1], data[2], data[3], data[4] } == ServerShutdownSquence)
                {
                    client.Close();
                    break;
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
                                        respString = ConfiguredRoutes[path].Invoke(reqArr[1]);
                                    }
                                    else
                                    {
                                        respString = HandleBadRequest();
                                    }
                                }
                                else if (reqRouteArr[0].Trim().ToUpper() == "GET")
                                {
                                    respString = HandleBadRequest();
                                }
                                else
                                {
                                    respString = HandleBadRequest();
                                }
                            }
                            else
                            {
                                respString = HandleNotFoundRequest();
                            }
                        }
                        else
                        {
                            respString = HandleBadRequest();
                        }
                    }
                    else
                    {
                        respString = HandleBadRequest();
                    }
                }
                else
                {
                    respString = HandleBadRequest();
                }
                client.Send(Encoding.UTF8.GetBytes(respString));
                client.Close();
                client.Dispose();
            }
        }

        private string HandleRegistrationRequest(string reqBody)
        {
            var reqmodel = JsonConvert.DeserializeObject<HTTPRequestModel>(reqBody, new JsonSerializerSettings() { Error = (o, e) =>
            {
                Logging.Error("Error parsing http registration request. " + e.ErrorContext.Error);
            }
            });
            if (reqmodel == null) return null;

            HTTPResponseModel respmodel = new HTTPResponseModel();
            respmodel.MacAddress = reqmodel.MacAddress;
            respmodel.HeartbeatRate = 10;
            respmodel.Status = "REG_APPROVED";
            respmodel.AuthToken = Helper.GenerateAuthToken(16);
            Dictionary<string, HTTPResponseModel> reply = new Dictionary<string, HTTPResponseModel>();
            reply.Add("success", respmodel);

            string header = "HTTP/1.1 200 OK\r\nServer: MMSHTTPServer\r\nAccept: application/json\r\nContent-Type: application/json; charset: UTF-8\r\n\r\n";
            string body = JsonConvert.SerializeObject(reply, new JsonSerializerSettings() { Error = (o, e) =>
            {
                Logging.Error("Error parsing http registration response. " + e.ErrorContext.Error);
            }
            });
            if (body == "") header = "HTTP/1.1 500 Internal Server Error\r\nServer: MMSHTTPServer";
            string response = header + body;
            return response;
        }

        private string HandleHeartbeatRequest(string reqBody)
        {
            return "";
        }

        private string HandleBadRequest()
        {
            string header = "HTTP/1.1 400 Bad Request\r\nServer: MMSHTTPServer\r\n";
            return header;
        }

        private string HandleNotFoundRequest()
        {
            string header = "HTTP/1.1 404 Not Found\r\nServer: MMSHTTPServer\r\n";
            return header;
        }
    }
}
