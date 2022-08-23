using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;

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
                stream.Flush();
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
                MemoryStream memstream = new MemoryStream(Encoding.UTF8.GetBytes(pem));
                RSAParameters param = DotNetUtilities.ToRSAParameters((RsaKeyParameters)new PemReader(new StreamReader(memstream)).ReadObject());
                RSACryptoServiceProvider encryptor = new RSACryptoServiceProvider();
                encryptor.ImportParameters(param);
                var retbytes = encryptor.Encrypt(data, true);
                Logging.Debug("TCPHandler: Data Encryption Successful.");
                return Encoding.UTF8.GetBytes(Convert.ToBase64String(retbytes));
            }
            catch (Exception e)
            {
                Logging.Debug("TCPHandler: Error During Data Encryption Using Public Key. " + e.Message);
                return null;
            }
        }
    }
}
