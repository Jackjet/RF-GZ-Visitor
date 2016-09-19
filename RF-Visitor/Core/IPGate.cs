using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RF_Visitor.Core
{
    class IPGate : IGate
    {
        TcpConnect tcp = null;
        bool connected = false;
        const int Delay = 1000;
        /// <summary>
        /// 连接网络设备
        /// </summary>
        /// <param name="ip"></param>
        public void Connect(string ip)
        {
            tcp = new TcpConnect();
            connected = tcp.Connect(ip);
            if (connected)
            {
                LogHelper.Info("网络继电器连接成功->" + ip);
            }
        }
        /// <summary>
        /// 入开闸
        /// </summary>
        public void OpenIn()
        {
            var data = OpenGate(1);
            tcp.Send(data);

            Thread.Sleep(Delay);

            data = OpenGate(0);
            tcp.Send(data);
        }
        /// <summary>
        /// 出开闸
        /// </summary>
        public void OpenOut()
        {
            var data = OpenGate(2);
            tcp.Send(data);

            Thread.Sleep(Delay);

            data = OpenGate(0);
            tcp.Send(data);
        }

        public void Disconnect()
        {
            tcp.DisConnect();
        }

        private static byte[] OpenGate(byte address)
        {
            List<byte> list = new List<byte>();
            list.Add(00);
            list.Add(01);

            list.Add(00);
            list.Add(00);

            list.Add(00);
            list.Add(08);

            list.Add(0xFF);
            list.Add(0x0F);

            list.Add(00);
            list.Add(0x64);

            list.Add(00);
            list.Add(02);

            list.Add(01);
            list.Add(address);

            var data = list.ToArray();
            return data;
        }
    }
}
