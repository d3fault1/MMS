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

        public void Start()
        {
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
                byte[] data = new byte[4096];
                var num = client.Receive(data);
                var datastr = Encoding.ASCII.GetString(data, 0, num);
                var dataarr = datastr.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (dataarr.Length != 2) continue;
                var reqbody = dataarr[1];
                var reqmodel = JsonConvert.DeserializeObject<HTTPRequestModel>(reqbody);
                string header = "HTTP/1.1 200 OK\nServer: MMSHTTPServer\nAccept: application/json\nContent-Type: application/json; charset: UTF-8\r\n\r\n";
                string body = GetRegistrationResponse(reqmodel);
                string response = header + body;
                client.Send(Encoding.ASCII.GetBytes(response));
                client.Close();
            }
        }

        private string GetRegistrationResponse(HTTPRequestModel reqmodel)
        {
            HTTPResponseModel response = new HTTPResponseModel();
            response.MacAddress = reqmodel.MacAddress;
            response.HeartbeatRate = 10;
            response.Status = "REG_APPROVED";
            response.AuthToken = Helper.GenerateAuthToken(16);
            Dictionary<string, HTTPResponseModel> reply = new Dictionary<string, HTTPResponseModel>();
            reply.Add("success", response);
            return JsonConvert.SerializeObject(reply);
        }
    }
}
