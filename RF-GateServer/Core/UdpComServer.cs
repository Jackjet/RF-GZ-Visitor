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
    class UdpComServer
    {
        private UdpClient server = null;
        private IPEndPoint remoteEndPoint = null;
        private Thread workThread = null;
        private bool isRun = true;
        public event MessageCommingEventHandler OnMessageInComming;

        public UdpComServer(int port)
        {
            server = new UdpClient(port, AddressFamily.InterNetwork);
        }

        public void Start()
        {
            workThread = new Thread(Listen);
            workThread.Start();
        }

        private void Listen()
        {
            while (isRun)
            {
                try
                {
                    var data = server.Receive(ref remoteEndPoint);
                    var remoteIp = remoteEndPoint.Address.ToString();
                    var qrcode = Encoding.UTF8.GetString(data, 0, data.Length);
                    ThreadPool.QueueUserWorkItem((s) =>
                    {
                        if (OnMessageInComming != null)
                            OnMessageInComming(remoteIp, qrcode);
                    });
                }
                catch
                {
                }
            }
        }

        public void Stop()
        {
            isRun = false;
            if (server != null)
            {
                server.Close();
            }
        }
    }

    public delegate void MessageCommingEventHandler(string ip, string qrcode);
}
