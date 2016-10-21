using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KonNaDSwitch
{
    /// <summary>
    /// TCP继电器
    /// </summary>
    public class TcpConnect
    {
        private TcpClient tcpclient = null;
        private NetworkStream stream = null;
        private IPAddress remoteAddress = null;
        private const int Port = 9876;

        public bool Connect(string ip)
        {
            try
            {
                remoteAddress = IPAddress.Parse(ip);
                tcpclient = new TcpClient();
                tcpclient.Connect(remoteAddress, Port);
                stream = tcpclient.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Info("网络继电器连接失败->" + ex.Message);
                return false;
            }
        }

        public bool Connected
        {
            get
            {
                return (tcpclient != null && tcpclient.Connected);
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            if (tcpclient.Connected)
            {
                if (stream.CanWrite)
                {
                    stream.Write(data, 0, data.Length);
                }
            }
        }
        /// <summary>
        /// 断开网络连接
        /// </summary>
        public void DisConnect()
        {
            if (stream != null)
            {
                stream.Close();
            }
            if (tcpclient != null)
            {
                tcpclient.Close();
            }
        }
    }
}
