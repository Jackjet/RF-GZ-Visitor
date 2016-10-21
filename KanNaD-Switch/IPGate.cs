using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KonNaDSwitch
{
    public class IPGate : IGate
    {
        private TcpConnect tcp = null;
        private bool connected = false;
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
        public void OpenIn(int delay)
        {
            var data = OpenGate(1);
            tcp.Send(data);

            Thread.Sleep(delay);
            data = OpenGate(0);
            tcp.Send(data);
        }
        /// <summary>
        /// 出开闸
        /// </summary>
        public void OpenOut(int delay)
        {
            var data = OpenGate(2);
            tcp.Send(data);

            Thread.Sleep(delay);
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
