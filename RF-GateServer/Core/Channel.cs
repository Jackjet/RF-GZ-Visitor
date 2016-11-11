using Common.NotifyBase;
using RF_GateServer.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RF_GateServer.Core
{
    /// <summary>
    /// 通道
    /// </summary>
    public class Channel : PropertyNotifyObject
    {
        private const int InTypeNo = 1;
        private const int OutTypeNo = 2;
        /// <summary>
        /// 序号
        /// </summary>
        public string Index
        {
            get { return this.GetValue(s => s.Index); }
            set { this.SetValue(s => s.Index, value); }
        }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName
        {
            get { return this.GetValue(s => s.AreaName); }
            set { this.SetValue(s => s.AreaName, value); }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string ChannelName
        {
            get { return this.GetValue(s => s.ChannelName); }
            set { this.SetValue(s => s.ChannelName, value); }
        }
        public string ItemId
        {
            get { return this.GetValue(s => s.ItemId); }
            set { this.SetValue(s => s.ItemId, value); }
        }
        /// <summary>
        /// 社区Id
        /// </summary>
        public string CommunityId
        {
            get { return this.GetValue(s => s.CommunityId); }
            set { this.SetValue(s => s.CommunityId, value); }
        }

        public string InIp
        {
            get { return this.GetValue(s => s.InIp); }
            set { this.SetValue(s => s.InIp, value); }
        }

        public string OutIp
        {
            get { return this.GetValue(s => s.OutIp); }
            set { this.SetValue(s => s.OutIp, value); }
        }

        public string GateIp
        {
            get { return this.GetValue(s => s.GateIp); }
            set { this.SetValue(s => s.GateIp, value); }
        }

        /// <summary>
        /// 入
        /// </summary>
        public IQRReader InReader
        {
            get; set;
        }
        /// <summary>
        /// 出
        /// </summary>
        public IQRReader OutReader
        {
            get; set;
        }
        /// <summary>
        /// 继电器
        /// </summary>
        public IGate Gate
        {
            get; set;
        }

        public async void Connect()
        {
            var task = Task.Factory.StartNew(() =>
            {
                if (InReader != null)
                {
                    var connect_in = InReader.Connect();
                    if (connect_in)
                    {
                        InReader.BeginRead(InReaderCallback);
                    }
                }

                if (OutReader != null)
                {
                    var connect_out = OutReader.Connect();
                    if (connect_out)
                    {
                        OutReader.BeginRead(OutReaderCallback);
                    }
                }
            });
            await task;
        }

        private void InReaderCallback(string ip, string qrcode)
        {
            Console.Out.WriteLine("in ip->" + ip + " qrcode->" + qrcode);
            Gate.In();
        }
        private void OutReaderCallback(string ip, string qrcode)
        {
            Console.Out.WriteLine("out ip->" + ip + " qrcode->" + qrcode);
            Gate.Out();
        }
    }
}
