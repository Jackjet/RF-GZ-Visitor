using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RF_GateServer.Gate
{
    class UdpSocket
    {
        private const int PORT = 5000;
        private UdpClient _udp = null;
        private IPEndPoint _remoteEndPoint = null;

        public UdpSocket(string ip)
        {
            _udp = new UdpClient();
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), PORT);
        }

        public void Send(byte[] data)
        {
            try
            {
                _udp.Send(data, data.Length, _remoteEndPoint);
            }
            catch (Exception ex)
            {
            }
        }

        public void Dispose()
        {
            if (_udp != null)
            {
                _udp.Close();
            }
        }
    }
}
