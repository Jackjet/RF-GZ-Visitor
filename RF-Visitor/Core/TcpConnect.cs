using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RF_Visitor.Core
{
    class TcpConnect
    {
        private TcpClient tcpclient = null;
        private NetworkStream stream = null;
        private IPAddress remoteAddress = null;

        private const int Port = 9877;

        public bool Connect(string ip)
        {
            try
            {
                remoteAddress = IPAddress.Parse(ip);
                tcpclient = new TcpClient();
                tcpclient.Connect(remoteAddress, Port);
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
