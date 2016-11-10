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
    public class Channel : Common.NotifyBase.PropertyNotifyObject
    {
        private const int InTypeNo = 1;
        private const int OutTypeNo = 2;
        /// <summary>
        /// 序号
        /// </summary>
        public string Index
        {
            get; set;
        }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName
        {
            get; set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string ChannelName
        {
            get; set;
        }
        public string ItemId
        {
            get; set;
        }
        /// <summary>
        /// 社区Id
        /// </summary>
        public string CommunityId
        {
            get; set;
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
