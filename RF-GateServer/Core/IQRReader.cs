using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_GateServer.Core
{
    public interface IQRReader
    {
        bool Connect(string ip, int port);

        void BeginRead(ReadBarCodeEventHandler callback);

        void DisConnect();
    }

    public delegate void ReadBarCodeEventHandler(string ip, string qrcode);
}
