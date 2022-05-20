using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Backend
{
    class Helper
    {
        private static Random rand = new Random();
        public static string GenerateAuthToken(int length)
        {
            byte[] charbyte = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int step = rand.Next(1, 3);
                switch (step)
                {
                    case 1:
                        charbyte[i] = (byte)rand.Next(48, 57);
                        break;
                    case 2:
                        charbyte[i] = (byte)rand.Next(65, 90);
                        break;
                }
            }
            return Encoding.ASCII.GetString(charbyte);
        }
    }
}
