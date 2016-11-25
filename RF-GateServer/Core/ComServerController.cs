using Common.NotifyBase;
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
    class ComServerController : PropertyNotifyObject
    {
        private int ComServerPort = 9876;
        private UdpComServer udpServer = null;
        private static ComServerController _instance = new ComServerController();

        private ComServerController()
        {
            ComServerPort = ConfigProfile.ListenPort;
            LivingDataCollection = new ObservableCollection<LivingData>();
        }

        public static ComServerController Current
        {
            get
            {
                return _instance;
            }
        }

        public bool IsRunning
        {
            get; set;
        }

        public ObservableCollection<Channel> Channels
        {
            get { return this.GetValue(s => s.Channels); }
            set { this.SetValue(s => s.Channels, value); }
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
                channel.Init();
            }

            udpServer = new UdpComServer(ComServerPort);
            udpServer.OnMessageInComming += UdpServer_OnMessageInComming;
            udpServer.Start();
            IsRunning = true;

            HeartBeat.Current.Start(Channels);
        }

        private void UdpServer_OnMessageInComming(object sender, MessageEventArgs e)
        {
            var ip = e.Ip;
            var qrcode = e.Data;
            var inchannel = Channels.FirstOrDefault(s => s.InIp == ip);
            if (inchannel != null)
            {
                inchannel.ChangeInState();
                if (e.IsQrcode)
                    inchannel.CheckIn(qrcode);
            }
            var outchannel = Channels.FirstOrDefault(s => s.OutIp == ip);
            if (outchannel != null)
            {
                outchannel.ChangeOutState();
                if (e.IsQrcode)
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
            IsRunning = false;
            udpServer?.Stop();
            udpServer = null;
            foreach (var channel in Channels)
            {
                channel.Stop();
            }
            HeartBeat.Current.Stop();
        }
    }
}
