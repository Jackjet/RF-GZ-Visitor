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
using System.Web.Script.Serialization;

namespace RF_Visitor.Core
{
    class HttpMethod
    {
        const string URL = "http://test.api.visitor.rfmember.net/api/building/open_door_qrcode?content={0}&item_id={1}&building_id={2}&type={3}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qrcode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static RFJsonResult Get(string qrcode, int type = 1)
        {
            var json = "";
            var error = "";
            json = Request(qrcode, type, out error);
            if (!error.IsEmpty())
            {
                LogHelper.Info("调用API服务异常->" + error);
                return new RFJsonResult();
            }
            else
            {
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                var result = serialize.Deserialize<RFJsonResult>(json);
                return result;
            }
        }

        private static string Request(string qrcode, int type, out string error)
        {
            var url = string.Format(URL,
             qrcode,
             ConfigPublic.TermID,
             ConfigPublic.BuildingID,
             type);

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
