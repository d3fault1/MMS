using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataModels
{
    class NodeCurrentStatus
    {
        public long NodeID { get; set; } = -1;
        public double Temperature { get; set; } = 0;
        public int ProcessorUsage { get; set; } = 0;
        public int DiskSpaceUsage { get; set; } = 0;
        public int RamUsage { get; set; } = 0;
        public TimeSpan TimeStamp { get; set; } = TimeSpan.Zero;
        public string VideoName { get; set; } = "";
        public int VideoNumber { get; set; } = 0;
        public string VideoStatus { get; set; } = "";
        public int Volume { get; set; } = 0;
        public double VideoDuration { get; set; } = 0;
        public TimeSpan Uptime { get; set; } = TimeSpan.Zero;
        public string Version { get; set; } = "";
    }
}
