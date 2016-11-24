using Common;
using Common.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_GateServer.Core
{
    static class ConfigProfile
    {
        public static int ListenPort = 0;
        public static bool AutoRun = false;
        public static string Host = "";

        private const string autoRun_key = "autoRun";
        private const string listenport_key = "listenPort";
        private const string host_key = "host";

        public static void ReadConfig()
        {
            LogHelper.Info("启动启动---------------------->");
            AutoRun = GetValue(autoRun_key) == "1";
            ListenPort = GetValue(listenport_key).ToInt32();
            Host = GetValue(host_key);
        }

        private static string GetValue(string key)
        {
            var keys = ConfigurationManager.AppSettings.AllKeys;
            var value = "";
            if (keys.Contains(key))
            {
                value = ConfigurationManager.AppSettings[key];
                LogHelper.Info(string.Format("读取配置项 {0}={1}", key, value));
            }
            else
            {
                LogHelper.Info(string.Format("读取配置项 {0} 失败", key));
            }
            return value;
        }
    }
}
