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

        public static int TypeID { get; set; }

        public static string ComPort { get; set; }


        public static void Init()
        {
            TermID = GetKey("item_id");
            BuildingID = GetKey("building_id");
            TypeID = GetKey("type").ToInt32();
            ComPort = GetKey("qrcomport");
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
