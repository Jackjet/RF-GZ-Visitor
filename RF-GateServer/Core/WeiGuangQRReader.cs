using Common;
using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RF_GateServer.Core
{
    public class WeiGuangQRReader : IQRReader
    {
        private TcpClient tcp = null;
        private Thread thread = null;
        private bool isRuning = false;
        private NetworkStream nws = null;

        private string ip;
        private int port;

        public WeiGuangQRReader(string ip, int port = 9876)
        {
            this.ip = ip;
            this.port = port;
        }

        public bool Connect()
        {
            try
            {
                IPAddress ipaddress = IPAddress.Parse(ip);
                tcp = new TcpClient();
                tcp.Connect(ipaddress, port);
                nws = tcp.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Info("阅读器连接失败->" + ex.Message);
                return false;
            }
        }

        public void DisConnect()
        {
            isRuning = false;
            nws.Close();
            tcp.Close();
            thread.Abort();
        }

        private ReadBarCodeEventHandler callback = null;
        private byte[] buffer = new byte[256];
        public void BeginRead(ReadBarCodeEventHandler callback)
        {
            this.callback = callback;
            this.isRuning = true;
            //thread = new Thread(Read);
            //thread.Start();
            nws.BeginRead(buffer, 0, buffer.Length, EndRead, nws);
        }

        private void EndRead(IAsyncResult ir)
        {
            var temp = (NetworkStream)ir.AsyncState;
            var len = temp.EndRead(ir);

            var code = System.Text.Encoding.UTF8.GetString(buffer, 0, len);
            callback?.BeginInvoke(ip, code, null, null);
            temp.BeginRead(buffer, 0, buffer.Length, EndRead, temp);
        }

        //private void Read()
        //{
        //    while (nws.CanRead && isRuning)
        //    {
        //        try
        //        {
        //            byte[] buffer = new byte[256];
        //            var len = nws.Read(buffer, 0, buffer.Length);
        //            var code = System.Text.Encoding.UTF8.GetString(buffer, 0, len );
        //            callback?.BeginInvoke(code, null, null);
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}
    }
}
