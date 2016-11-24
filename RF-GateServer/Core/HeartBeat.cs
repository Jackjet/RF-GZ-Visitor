using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_GateServer.Core
{
    class HeartBeat
    {
        private ObservableCollection<Channel> Channels = null;
        private bool isRunning = false;
        private const int Interval = 60 * 1000;
        public void Check()
        {
            isRunning = true;
            while (isRunning)
            {
                foreach (var channel in Channels)
                {

                }
            }
        }

        public void Stop()
        {
            isRunning = false;
        }

        private void CheckOk(Channel channel)
        {
            if (channel.InIp.IsEmpty())
            {
                var ts = DateTime.Now - channel.InLastTime.Value;
                if (ts.TotalSeconds > Interval)
                {
                    channel.SetInError();
                }
            }
            if (channel.OutIp.IsEmpty())
            {
                var ts = DateTime.Now - channel.OutLastTime.Value;
                if (ts.TotalSeconds > Interval)
                {
                    channel.SetOutError();
                }
            }
        }
    }
}
