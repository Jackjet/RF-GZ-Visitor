using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Common.NotifyBase;

namespace RF_Visitor.Core
{
    class Core : PropertyNotifyObject
    {
        private IGate gate = null;
        private SerialQRCodeReader readerIn = null;
        private SerialQRCodeReader readerOut = null;

        public string VerfiyMessage
        {
            get { return this.GetValue(s => s.VerfiyMessage); }
            set { this.SetValue(s => s.VerfiyMessage, value); }
        }

        public void Init()
        {
            ConfigPublic.Init();

            InitReader();
            InitGate();

            VerfiyMessage = "请刷二维码通行";
        }

        private void InitReader()
        {
            if (!ConfigPublic.InComPort.IsEmpty())
            {
                readerIn = new SerialQRCodeReader();
                if (readerIn.Open(ConfigPublic.InComPort))
                {
                    readerIn.SetQRCodeCallback(QRReaderCallback_In);
                    Log("[入]设备串口打开->" + ConfigPublic.InComPort);
                }
            }
            else
            {
                Log("[入]设备串口未配置");
            }

            if (!ConfigPublic.OutComPort.IsEmpty())
            {
                readerOut = new SerialQRCodeReader();
                if (readerOut.Open(ConfigPublic.OutComPort))
                {
                    readerOut.SetQRCodeCallback(QRReaderCallback_Out);
                    Log("[出]设备串口打开->" + ConfigPublic.OutComPort);
                }
            }
            else
            {
                Log("[出]设备串口未配置");
            }
        }

        private void InitGate()
        {
            if (ConfigPublic.OpenGateType == "1")
            {
                //串口
                gate = new SerialGate();
                gate.Connect(ConfigPublic.GateComPort);
            }
            else
            {
                //网络
                gate = new IPGate();
                gate.Connect(ConfigPublic.GateIP);
            }
        }

        public void QRReaderCallback_In(string code)
        {
            var result = HttpMethod.Get(code, 1);
            if (result.content && result.success)
            {
                Log("入->{0}", code);
                VerfiyMessage = "入->请通行";
                gate.OpenIn();
            }
            else
            {
                VerfiyMessage = "入->未授权";
            }
        }

        public void QRReaderCallback_Out(string code)
        {
            var result = HttpMethod.Get(code, 2);
            if (result.content && result.success)
            {
                Log("出->{0}", code);
                VerfiyMessage = "出->请通行";
                gate.OpenOut();
            }
            else
            {
                VerfiyMessage = "出->未授权";
            }
        }

        public void Dispose()
        {
            if (gate != null)
            {
                gate.Disconnect();
            }

            if (readerIn != null)
            {
                readerIn.Dispose();
            }

            if (readerOut != null)
            {
                readerOut.Dispose();
            }
        }

        private static void Log(string str, params string[] arg)
        {
            LogHelper.Info(str, arg);
        }
    }
}
