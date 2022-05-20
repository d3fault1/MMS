using System;
using System.IO;

namespace MMS.Backend
{
    static class Logging
    {
        static string logfile = "applog.log";
        static FileStream logstream;
        static StreamWriter logwriter;
        public static void Initialize()
        {
            logstream = new FileStream(logfile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            logwriter = new StreamWriter(logstream);
        }

        public static void Release()
        {
            logwriter?.Close();
            logwriter?.Dispose();
            logstream?.Dispose();
        }

        public static void Info(string message)
        {
            logwriter?.WriteLine($"[Info]\t{DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")}\t{message}");
        }

        public static void Error(string message)
        {
            logwriter?.WriteLine($"[Error]\t{DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")}\t{message}");
        }
    }
}
