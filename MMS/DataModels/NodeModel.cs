using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataModels
{
    class NodeModel
    {
        public long ID { get; set; } = -1;
        public string Name { get; set; } = "";
        public string NodeName { get; set; } = "";
        public string Description { get; set; } = "";
        public string IP { get; set; } = "";
        public bool IsActive { get; set; } = false;
        public bool IsConfig { get; set; } = false;
        public string OSType { get; set; } = "";
        public string MacAddress { get; set; } = "";
        public int Port { get; set; } = -1;
        public string RegKey { get; set; } = "";
        public string OSName { get; set; } = "";
        public string OSArchitecture { get; set; } = "";
        public double TotalDiskSpace { get; set; } = 0;
        public double TotalCPU { get; set; } = 0;
        public double TotalRam { get; set; } = 0;
        public long ExhibitID { get; set; } = -1;
        public long ZoneID { get; set; } = -1;
        public long FloorID { get; set; } = -1;
        public string ContentMetadata { get; set; } = "";
        public string PEMFile { get; set; } = "";
        public int HeartbeatRate { get; set; } = 0;
        public string Image { get; set; } = "";
        public bool IsOnline { get; set; } = false;
        public int TotalVideos { get; set; } = 0;
        public string VideoList { get; set; } = "";
        public int SequenceID { get; set; } = -1;
        public bool IsAudioGuide { get; set; } = false;
        public string Category { get; set; } = "";
        public bool IsControlPanel { get; set; } = false;
        public NodeCurrentStatus CurrentStatus { get; set; } = new NodeCurrentStatus();
    }
}
