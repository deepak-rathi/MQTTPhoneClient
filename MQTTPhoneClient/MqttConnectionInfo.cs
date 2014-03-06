using System.Runtime.Serialization;

namespace MQTTPhoneClient
{
    [DataContract]
    public class MqttConnectionInfo
    {
        [DataMember]
        public string ClientName { get; set; }
        [DataMember]
        public string HostName { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
