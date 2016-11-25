using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_GateServer.DataManager
{
    class ChannelDisconnectModel
    {
        public string Name { get; set; }

        public string Ip { get; set; }

        public string ChannelType { get; set; }

        public DateTime DisconnectTime { get; set; }

        public DateTime? ConnectTime { get; set; }
    }
}
