using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_GateServer.Core.WebAPI
{
    public class RFJsonResult
    {
        public int status { get; set; }

        public string message { get; set; }

        public object data { get; set; }
    }
}
