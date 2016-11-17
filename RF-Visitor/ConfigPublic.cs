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
    /// <summary>
    /// 运行配置
    /// </summary>
    static class ConfigPublic
    {
        public static string TermID { get; set; }

        public static string BuildingID { get; set; }

        public static string InComPort { get; set; }

        public static string OutComPort { get; set; }

        public static string OpenGateType { get; set; }

        public static string GateComPort { get; set; }

        public static string GateIP { get; set; }

        public static int Delay { get; set; }

        public static string Host { get; set; }

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

            Delay = GetKey("Delay").ToInt32();
            if (Delay == 0)
                Delay = 5000;

            var hostPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "host.txt");
            if (System.IO.File.Exists(hostPath))
                Host = System.IO.File.ReadAllText(hostPath);
            else
                Host = "http://127.0.0.1";
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
