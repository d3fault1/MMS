using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataModels
{
    class ViewDevicesModel
    {
        public string Name { get; set; } = "";
        public string Mac { get; set; } = "";
        public string IP { get; set; } = "";
        public string Version { get; set; } = "";
        public string Floor { get; set; } = "";
        public string Zone { get; set; } = "";
        public string Exhibit { get; set; } = "";
        public DeviceStatus Status { get; set; } = DeviceStatus.Offline;
    }

    enum DeviceStatus
    {
        Online,
        Offline,
        NotConfigured,
        AwaitingRegistration
    }
}
