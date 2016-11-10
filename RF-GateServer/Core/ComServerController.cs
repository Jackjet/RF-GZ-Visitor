using RF_GateServer.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RF_GateServer.Core
{
    class ComServerController
    {
        private const int ComServerPort = 9876;
        private List<Channel> channelList = null;

        public void Run()
        {
            channelList = MapReader.Read();
            foreach (Channel channel in channelList)
            {
                channel.Connect();
            }
        }
    }
}
