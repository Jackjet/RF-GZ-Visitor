using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_Visitor
{
    class RFJsonResult
    {
        public bool success { get; set; }

        public bool content { get; set; }

        public bool hasError { get; set; }

        public string httpStatus { get; set; }
    }
}
