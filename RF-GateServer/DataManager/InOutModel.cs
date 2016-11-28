using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_GateServer.DataManager
{
    class InOutModel
    {
        public string Name { get; set; }

        public string Ip { get; set; }

        public string ChannelType { get; set; }

        public string QRCode { get; set; }

        public string Status { get; set; }

        public string ElapseTime { get; set; }

        public DateTime CheckTime { get; set; }
    }
}
