using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataModels
{
    class NodeLogModel
    {
        public long NodeID { get; set; } = -1;
        public double Temperature { get; set; } = 0;
        public TimeSpan Uptime { get; set; } = TimeSpan.Zero;
        public int ProcessorUsage { get; set; } = 0;
        public int DiskSpaceUsage { get; set; } = 0;
        public int RamUsage { get; set; } = 0;
        public string Version { get; set; } = "";
    }
}
