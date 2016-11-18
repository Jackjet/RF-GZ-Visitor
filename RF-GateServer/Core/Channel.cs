using Common;
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

        public Channel()
        {
            InLastTime = "未知";
            OutLastTime = "未知";
        }

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
        public string Area
        {
            get { return this.GetValue(s => s.Area); }
            set { this.SetValue(s => s.Area, value); }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return this.GetValue(s => s.Name); }
            set { this.SetValue(s => s.Name, value); }
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

        public string InLastTime
        {
            get { return this.GetValue(s => s.InLastTime); }
            set { this.SetValue(s => s.InLastTime, value); }
        }

        public string OutLastTime
        {
            get { return this.GetValue(s => s.OutLastTime); }
            set { this.SetValue(s => s.OutLastTime, value); }
        }

        public async void Connect()
        {
            var task = Task.Factory.StartNew(() =>
            {
                //if (InReader != null)
                //{
                //    var connect_in = InReader.Connect();
                //    if (connect_in)
                //    {
                //        InReader.BeginRead(InReaderCallback);
                //    }
                //}

                //if (OutReader != null)
                //{
                //    var connect_out = OutReader.Connect();
                //    if (connect_out)
                //    {
                //        OutReader.BeginRead(OutReaderCallback);
                //    }
                //}
            });
            await task;
        }

        public void SetGate(IGate gate)
        {
            this.Gate = gate;
        }

        private void InReaderCallback(string ip, string qrcode)
        {
            Console.Out.WriteLine("in ip->" + ip + " qrcode->" + qrcode);
            CheckIn(qrcode);
        }
        private void OutReaderCallback(string ip, string qrcode)
        {
            Console.Out.WriteLine("out ip->" + ip + " qrcode->" + qrcode);
            CheckOut(qrcode);
        }

        public void CheckIn(string qrcode)
        {
            InLastTime = DateTime.Now.ToStandard();

            var data = GetLivingData(this.InIp, qrcode, 1, 123);
            ComServerController.Instance.AddLivingData(data);
            Gate.In();
        }

        public void CheckOut(string qrcode)
        {
            OutLastTime = DateTime.Now.ToStandard();

            var data = GetLivingData(this.OutIp, qrcode, 0, 456);
            ComServerController.Instance.AddLivingData(data);
            Gate.Out();
        }

        public void Stop()
        {

        }

        private LivingData GetLivingData(string ip, string qrcode, int status, int elapsed)
        {
            LivingData data = new LivingData
            {
                Index = this.Index,
                Area = this.Area,
                Name = this.Name,
                ItemId = this.ItemId,
                CommunityId = this.CommunityId,
                IP = ip,
                Status = status == 1 ? "授权" : "未授权",
                Elapsed = elapsed.ToString(),
                QRCode = qrcode,
                DateTime = DateTime.Now.ToStandard()
            };
            return data;
        }
    }
}
