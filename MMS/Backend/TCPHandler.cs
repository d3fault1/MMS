using System;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace MMS.Backend
{
    class TCPHandler
    {
        static TCPHandler _Instance = new TCPHandler();
        public static TCPHandler Instance { get => _Instance; }

        public bool Send(byte[] data, string ip, int port)
        {
            try
            {
                var client = new TcpClient();
                client.SendTimeout = 1000;
                Logging.Debug("TCPHandler: Connecting to TCP.");
                client.Connect(ip, port);
                var stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                client.Close();
                client.Dispose();
                Logging.Debug("TCPHandler: Data Sent Successfully.");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug("TCPHandler: Error Sending Data. " + e.Message);
                return false;
            }

        }

        public byte[] EncryptFromPublicKey(byte[] data, string pem)
        {
            try
            {
                Logging.Debug("TCPHandler: Encrypting Data with Public Key.");
                RSACryptoServiceProvider encryptor = new RSACryptoServiceProvider();
                RSAParameters param = encryptor.ExportParameters(false);
                param.Modulus = Convert.FromBase64String(pem.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "").Replace("\n", ""));
                encryptor.ImportParameters(param);
                var retbytes = encryptor.Encrypt(data, false);
                Logging.Debug("TCPHandler: Data Encryption Successful.");
                return retbytes;
            }
            catch (Exception e)
            {
                Logging.Debug("TCPHandler: Error During Data Encryption Using Public Key. " + e.Message);
                return null;
            }
        }
    }
}
