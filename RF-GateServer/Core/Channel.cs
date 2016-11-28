using Common;
using Common.NotifyBase;
using RF_GateServer.Core.WebAPI;
using RF_GateServer.DataManager;
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
        private const string ChannelIn = "1";
        private const string ChannelOut = "2";

        public Channel()
        {
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

        public string CommunicateInState
        {
            get { return this.GetValue(s => s.CommunicateInState); }
            set { this.SetValue(s => s.CommunicateInState, value); }
        }

        public string CommunicateOutState
        {
            get { return this.GetValue(s => s.CommunicateOutState); }
            set { this.SetValue(s => s.CommunicateOutState, value); }
        }

        #region connect
        ///// <summary>
        ///// 入
        ///// </summary>
        //public IQRReader InReader
        //{
        //    get; set;
        //}
        ///// <summary>
        ///// 出
        ///// </summary>
        //public IQRReader OutReader
        //{
        //    get; set;
        //} 
        #endregion

        /// <summary>
        /// 继电器
        /// </summary>
        public IGate Gate
        {
            get; set;
        }

        public DateTime? InLastTime
        {
            get { return this.GetValue(s => s.InLastTime); }
            set { this.SetValue(s => s.InLastTime, value); }
        }

        public DateTime? OutLastTime
        {
            get { return this.GetValue(s => s.OutLastTime); }
            set { this.SetValue(s => s.OutLastTime, value); }
        }

        public void SetGate(IGate gate)
        {
            this.Gate = gate;
        }

        public void Init()
        {
            if (!InIp.IsEmpty())
            {
                InLastTime = null;
                CommunicateInState = ChannelState.State_Unknow;
            }

            if (!OutIp.IsEmpty())
            {
                OutLastTime = null;
                CommunicateOutState = ChannelState.State_Unknow;
            }
        }

        public void Stop()
        {
            if (!InIp.IsEmpty())
                CommunicateInState = ChannelState.State_Stop;

            if (!OutIp.IsEmpty())
                CommunicateOutState = ChannelState.State_Stop;
        }

        public void VerifyIn(string qrcode)
        {
            var elapseTime = 0;
            var status = 0;
            var verfiy = HttpMethod.Get(qrcode, CommunityId, ItemId, ChannelIn, out elapseTime);
            status = verfiy.status == 200 ? 1 : 0;

            var livingRecord = GetLivingData(this.InIp, qrcode, status, elapseTime);
            ComServerController.Current.AddLivingData(livingRecord);

            var inoutRecord = GetInOutData(InIp, qrcode, "入", status, elapseTime);
            SQLite.Current.InOut(inoutRecord);

            if (status == 1)
            {
                Gate.In();
            }
        }

        public void VerifyOut(string qrcode)
        {
            var elapseTime = 0;
            var status = 0;

            var verfiy = HttpMethod.Get(qrcode, CommunityId, ItemId, ChannelIn, out elapseTime);
            status = verfiy.status == 200 ? 1 : 0;

            var livingRecord = GetLivingData(this.OutIp, qrcode, status, elapseTime);
            ComServerController.Current.AddLivingData(livingRecord);

            var inoutRecord = GetInOutData(OutIp, qrcode, "出", status, elapseTime);
            SQLite.Current.InOut(inoutRecord);

            if (status == 1)
            {
                Gate.Out();
            }
        }

        public void ChangeInState()
        {
            if (CommunicateInState == ChannelState.State_Error)
            {
                SQLite.Current.Connect(InIp);
            }
            InLastTime = DateTime.Now;
            CommunicateInState = ChannelState.State_Ok;
        }

        public void ChangeOutState()
        {
            if (CommunicateOutState == ChannelState.State_Error)
            {
                SQLite.Current.Connect(OutIp);
            }
            OutLastTime = DateTime.Now;
            CommunicateOutState = ChannelState.State_Ok;
        }

        private void SetInError()
        {
            CommunicateInState = ChannelState.State_Error;
            SQLite.Current.Disconnect(Name, InIp, ChannelType.In);
        }

        private void SetOutError()
        {
            CommunicateOutState = ChannelState.State_Error;
            SQLite.Current.Disconnect(Name, OutIp, ChannelType.Out);
        }

        public void CheckInState(int interval)
        {
            if (InLastTime.HasValue)
            {
                if (CommunicateInState != ChannelState.State_Stop && CommunicateInState != ChannelState.State_Error)
                {
                    var ts = DateTime.Now - InLastTime.Value;
                    if (ts.TotalSeconds > interval)
                    {
                        SetInError();
                    }
                }
            }
            else
            {
                SetInError();
            }
        }

        public void CheckOutState(int interval)
        {
            if (OutLastTime.HasValue)
            {
                if (CommunicateOutState != ChannelState.State_Stop && CommunicateOutState != ChannelState.State_Error)
                {
                    var ts = DateTime.Now - OutLastTime.Value;
                    if (ts.TotalSeconds > interval)
                    {
                        SetOutError();
                    }
                }
            }
            else
            {
                SetOutError();
            }
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
                Ip = ip,
                Status = status == 1 ? "授权" : "未授权",
                Elapsed = elapsed.ToString(),
                QRCode = qrcode,
                DateTime = DateTime.Now.ToStandard()
            };
            return data;
        }

        private InOutModel GetInOutData(string ip, string qrcode, string type, int status, int elapsed)
        {
            InOutModel data = new InOutModel
            {
                Name = this.Name,
                Ip = ip,
                ChannelType = type,
                Status = status == 1 ? "授权" : "未授权",
                ElapseTime = elapsed.ToString(),
                QRCode = qrcode,
                CheckTime = DateTime.Now
            };
            return data;
        }
    }
}
