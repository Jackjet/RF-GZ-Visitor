using Common;
using Common.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace RF_GateServer.Core.WebAPI
{
    class HttpMethod
    {
        const string test = "?content={0}&community_id={1}&item_id={2}&type={3}";
        const string URL = "/api/community/qrcode/check";

        private static string PreLinkUrl(string qrcode, string communityId, string itemId, string type)
        {
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("content", qrcode.UrlEncode());
            parms.Add("community_id", communityId);
            parms.Add("item_id", itemId);
            parms.Add("type", type);
            return parms.LinkUrl();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qrcode"></param>
        /// <param name="communityId"></param>
        /// <param name="itemId"></param>
        /// <param name="type"></param>
        /// <param name="elapseTime"></param>
        /// <returns></returns>
        public static RFJsonResult Get(string qrcode, string communityId, string itemId, string type, out int elapseTime)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var json = "";
            var error = "";
            var url = ConfigProfile.Host + "?" + PreLinkUrl(qrcode, communityId, itemId, type);
            var result = new RFJsonResult();
            json = Request(url, out error);
            if (!error.IsEmpty())
            {
                LogHelper.Info("调用API服务异常->" + error);
            }
            else
            {
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                result = serialize.Deserialize<RFJsonResult>(json);
            }
            sw.Stop();
            elapseTime = (int)sw.ElapsedMilliseconds;
            return result;
        }

        private static string Request(string url, out string error)
        {
            var json = "";
            error = "";
            Stopwatch sw = Stopwatch.StartNew();
            WebRequest wr = WebRequest.Create(url);
            try
            {
                var response = wr.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    json = sr.ReadToEnd();
                }
                if (string.IsNullOrEmpty(json))
                {
                    error = "返回结果为空";
                }
            }
            catch (Exception ex)
            {
                error = "API调用失败->" + ex.Message;
            }
            finally
            {
                sw.Stop();
                LogHelper.Info("call api->" + sw.ElapsedMilliseconds);
            }
            return json;
        }
    }
}
