using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTPhoneClient
{
    internal class MqttConnectionInfo
    {
        public string ClientName
        {
            get { return "Phone8Client"; }
        }
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
