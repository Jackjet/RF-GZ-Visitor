using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_Visitor.Core
{
    interface IGate
    {
        void Connect(string ip);

        void OpenIn();

        void OpenOut();

        void Disconnect();
    }
}
