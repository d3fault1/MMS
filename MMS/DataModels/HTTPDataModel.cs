using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MMS.DataModels
{
    class HTTPRequestModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("node_name")]
        public string NodeName { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("mac_addr")]
        public string MacAddress { get; set; }
        [JsonProperty("ip")]
        public string IP { get; set; }
        [JsonProperty("port")]
        public string Port { get; set; }
        [JsonProperty("unique_reg_code")]
        public string UniqueRegCode { get; set; }
        [JsonProperty("os_type")]
        public string OSType { get; set; }
        [JsonProperty("os_name")]
        public string OSName { get; set; }
        [JsonProperty("os_arch")]
        public string OSArch { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    class HTTPResponseModel
    {
        [JsonProperty("mac_addr")]
        public string MacAddress { get; set; }
        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("heartbeat_rate")]
        public int HeartbeatRate { get; set; }
    }
}
