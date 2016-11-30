using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RF_GateServer.Core
{
    /// <summary>
    /// 心跳包检测
    /// </summary>
    class HeartBeat
    {
        private bool isRunning = false;
        private Thread workThread = null;

        private static int check_Interval = 0;
        private static int heartbeat_Interval = 0;

        private static HeartBeat _current = new HeartBeat();

        private HeartBeat()
        {
            check_Interval = ConfigProfile.checkInterval * 1000;
            heartbeat_Interval = ConfigProfile.heartBeatInterval * 1000;
        }

        public static HeartBeat Current
        {
            get
            {
                return _current;
            }
        }

        public void Start(IEnumerable<Channel> channels)
        {
            if (isRunning)
                return;

            workThread = new Thread(() =>
            {
                isRunning = true;
                while (isRunning)
                {
                    foreach (var channel in channels)
                    {
                        Thread.Sleep(check_Interval);
                        CheckChannel(channel);
                    }
                }
            });
            workThread.Start();
        }

        public void Stop()
        {
            isRunning = false;
            workThread?.Abort();
            workThread = null;
        }

        private void CheckChannel(Channel channel)
        {
            if (!channel.InIp.IsEmpty())
            {
                channel.CheckInState(heartbeat_Interval);
            }
            if (!channel.OutIp.IsEmpty())
            {
                channel.CheckOutState(heartbeat_Interval);
            }
        }
    }
}
