using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_GateServer.Core
{
    class MessageEventArgs : EventArgs
    {
        public string Ip { get; set; }

        public bool IsHeart { get; set; }

        public bool IsQrcode { get; set; }

        public string Data { get; set; }
    }

    class ChannelState
    {
        public const string State_Unknow = "未知";
        public const string State_Ok = "正常";
        public const string State_Error = "异常";
        public const string State_Stop = "停止";
    }

    class ChannelType
    {
        public const string In = "入";
        public const string Out = "出";
    }
}
