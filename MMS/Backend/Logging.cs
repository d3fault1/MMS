using System;
using System.IO;

namespace MMS.Backend
{
    static class Logging
    {
        static string logfile = "applog.log";
        static bool debugMode = false;
        static FileStream logstream;
        static StreamWriter logwriter;
        public static void Initialize()
        {
            logstream = new FileStream(logfile, FileMode.Append, FileAccess.Write);
            logwriter = new StreamWriter(logstream);
            logwriter.AutoFlush = true;
            logwriter.WriteLine();
            logwriter.WriteLine("------------------ Logger Startup ------------------");
            logwriter.WriteLine();
        }

        public static void Release()
        {
            logwriter?.Close();
            logwriter?.Dispose();
            logstream?.Dispose();
        }

        public static void EnableDebugLogging()
        {
            debugMode = true;
        }

        public static void DisableDebugLogging()
        {
            debugMode = false;
        }

        public static void Info(string message)
        {
            logwriter?.WriteLine($"[Info]\t{DateTime.Now.TimeOfDay:hh\\:mm\\:ss\\.fff}\t{message}");
        }

        public static void Debug(string message)
        {
            if (debugMode) logwriter?.WriteLine($"[Debug]\t{DateTime.Now.TimeOfDay:hh\\:mm\\:ss\\.fff}\t{message}");
        }

        public static void Error(string message)
        {
            logwriter?.WriteLine($"[Error]\t{DateTime.Now.TimeOfDay:hh\\:mm\\:ss\\.fff}\t{message}");
        }
    }
}
