using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_Visitor.Core
{
    /// <summary>
    /// 授权返回
    /// </summary>
    public class RFJsonResult
    {
        public int status { get; set; }

        public string message { get; set; }

        public object data { get; set; }
    }
}
