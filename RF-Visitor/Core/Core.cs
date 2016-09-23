using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Common.NotifyBase;
using System.Windows.Input;
using System.Threading;

namespace RF_Visitor.Core
{
    class Core : PropertyNotifyObject
    {
        private IGate gate = null;
        private bool isStop = false;
        private SerialQRCodeReader readerIn = null;
        private SerialQRCodeReader readerOut = null;

        const string OKImage = "yes.png";
        const string NOImage = "no.png";
        const string WelCome = "请刷二维码通行";

        private FuncTimeout timeout = null;
        public string VerfiyMessage
        {
            get { return this.GetValue(s => s.VerfiyMessage); }
            set { this.SetValue(s => s.VerfiyMessage, value); }
        }

        public string SystemDateTime
        {
            get { return this.GetValue(s => s.SystemDateTime); }
            set { this.SetValue(s => s.SystemDateTime, value); }
        }

        public string StateImage
        {
            get { return this.GetValue(s => s.StateImage); }
            set { this.SetValue(s => s.StateImage, value); }
        }

        public void Init()
        {
            ConfigPublic.Init();

            InitReader();
            InitGate();

            VerfiyMessage = WelCome;
            timeout = new FuncTimeout();
            Task.Factory.StartNew(() =>
            {
                while (!isStop)
                {
                    SystemDateTime = DateTime.Now.ToStandard();
                    Thread.Sleep(1000);
                }
            });
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
            try
            {
                Log("入->{0}", code);
                var result = HttpMethod.Get(code, 1);
                if (result.content && result.success)
                {
                    VerfiyMessage = "入->请通行";
                    StateImage = OKImage;
                    gate.OpenIn();
                }
                else
                {
                    VerfiyMessage = "入->未授权";
                    StateImage = NOImage;
                }
                Welcome();
            }
            catch (Exception ex)
            {
                LogHelper.Info("入异常->" + ex.Message);
            }
        }

        public void QRReaderCallback_Out(string code)
        {
            Log("出->{0}", code);
            var result = HttpMethod.Get(code, 2);
            if (result.content && result.success)
            {
                VerfiyMessage = "出->请通行";
                StateImage = OKImage;
                gate.OpenOut();
            }
            else
            {
                VerfiyMessage = "出->未授权";
                StateImage = NOImage;
            }
            Welcome();
        }

        private void Welcome()
        {
            timeout.StartOnce(2000, () =>
            {
                StateImage = "";
                VerfiyMessage = WelCome;
            });
        }

        public void Dispose()
        {
            isStop = true;
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
