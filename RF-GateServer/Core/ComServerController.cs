using RF_GateServer.Gate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RF_GateServer.Core
{
    class ComServerController
    {
        private UdpComServer udpServer = null;
        private const int ComServerPort = 9876;
        public ObservableCollection<Channel> Channels = null;
        private static ComServerController _instance = new ComServerController();

        private ComServerController()
        {
            LivingDataCollection = new ObservableCollection<LivingData>();
        }

        public static ComServerController Instance
        {
            get
            {
                return _instance;
            }
        }

        public ObservableCollection<LivingData> LivingDataCollection
        {
            get;
            set;
        }


        public void Run()
        {
            Channels = MapReader.Read();
            foreach (var channel in Channels)
            {
                channel.SetGate(new MegviiGate(channel.GateIp));
            }

            udpServer = new Core.UdpComServer(ComServerPort);
            udpServer.OnMessageInComming += UdpServer_OnMessageInComming;
            udpServer.Start();
        }

        private void UdpServer_OnMessageInComming(string ip, string qrcode)
        {
            var inchannel = Channels.FirstOrDefault(s => s.InIp == ip);
            if (inchannel != null)
            {
                inchannel.CheckIn(qrcode);
            }
            var outchannel = Channels.FirstOrDefault(s => s.OutIp == ip);
            if (outchannel != null)
            {
                outchannel.CheckOut(qrcode);
            }
        }

        public void RemoveChannel(Channel channel)
        {
            channel.Stop();
            Channels.Remove(channel);
        }

        private int index = 1;
        public void AddLivingData(LivingData data)
        {
            if (index == 100)
            {
                LivingDataCollection.Clear();
            }

            data.Index = index.ToString("d3");
            Application.Current.Dispatcher.Invoke(() =>
            {
                LivingDataCollection.Insert(0, data);
            });
            index++;
        }

        public void Stop()
        {
            udpServer?.Stop();
        }
    }
}
