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

        void Write();

        void BeginRead(Action<string> callbakc);

        void DisConnect();
    }
}
