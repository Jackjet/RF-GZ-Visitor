using Common.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace RF_Visitor
{
    class ConfigPublic
    {
        public static string TermID { get; set; }

        public static string BuildingID { get; set; }

        public static string InComPort { get; set; }

        public static string OutComPort { get; set; }

        public static string OpenGateType { get; set; }

        public static string GateComPort { get; set; }

        public static string GateIP { get; set; }

        /// <summary>
        /// 读取配置参数
        /// </summary>
        public static void Init()
        {
            TermID = GetKey("item_id");
            BuildingID = GetKey("building_id");

            InComPort = GetKey("InComPort");
            OutComPort = GetKey("OutComPort");

            OpenGateType = GetKey("OpenGateType");
            if (OpenGateType == "1")
                GateComPort = GetKey("GateComPort");
            else
                GateIP = GetKey("GateIP");
        }

        private static string GetKey(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                var val = ConfigurationManager.AppSettings[key];
                LogHelper.Info(string.Format("参数[{0}]={1}", key, val));
                return val;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
