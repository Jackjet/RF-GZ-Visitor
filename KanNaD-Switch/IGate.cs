using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonNaDSwitch
{
    public interface IGate
    {
        void Connect(string ip);

        void OpenIn(int delay);

        void OpenOut(int delay);

        void Disconnect();
    }
}
