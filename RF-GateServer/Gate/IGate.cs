using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_GateServer.Gate
{
    public interface IGate
    {
        void In();

        void Out();
    }
}
