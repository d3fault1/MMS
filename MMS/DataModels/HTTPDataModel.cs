using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MMS.DataModels
{
    class HTTPRegistrationReqModel
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
        public int Port { get; set; }
        [JsonProperty("encrypted_port")]
        public int SecurePort { get; set; }
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
        [JsonProperty("pem_file")]
        public string PEMFile { get; set; }
    }

    class HTTPRegistrationAckModel : HTTPResponseModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    class HTTPRegistrationRespModel : HTTPResponseModel
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

    class HTTPHeartbeatReqModel
    {
        [JsonProperty("mac_addr")]
        public string MacAddress { get; set; }
        [JsonProperty("temparature")]
        public double Temperature { get; set; }
        [JsonProperty("cpu_usage")]
        public int ProcessorUsage { get; set; }
        [JsonProperty("disc_space_usage")]
        public int DiskSpaceUsage { get; set; }
        [JsonProperty("ram_usage")]
        public int RamUsage { get; set; }
        [JsonProperty("current_timestamp")]
        public double TimeStamp { get; set; }
        [JsonProperty("current_video_name")]
        public string VideoName { get; set; }
        [JsonProperty("current_video_number")]
        public int VideoNumber { get; set; }
        [JsonProperty("current_video_status")]
        public string VideoStatus { get; set; }
        [JsonProperty("current_volume")]
        public int Volume { get; set; }
        [JsonProperty("vduration")]
        public double? VideoDuration { get; set; }
        [JsonProperty("totalVideos")]
        public int TotalVideos { get; set; }
        [JsonProperty("video_list")]
        public string VideoList { get; set; }
        [JsonProperty("uptime")]
        public double Uptime { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    class HTTPHeartbeatAckModel : HTTPResponseModel
    {

    }

    class HTTPGeneralRespModel
    {
        [JsonProperty("success")]
        public HTTPResponseModel ResponseModel { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("error")]
        public bool IsError { get; set; }
    }

    class HTTPCommandStatusReqModel
    {
        [JsonProperty("command_log_id")]
        public int ID { get; set; }
        [JsonProperty("mac_addr")]
        public string MacAddress { get; set; }
        [JsonProperty("command_status")]
        public string Status { get; set; }
        [JsonProperty("command_message")]
        public string Message { get; set; }
    }

    class HTTPCommandStatusAckModel : HTTPResponseModel
    {

    }

    interface HTTPResponseModel
    {

    }
}
