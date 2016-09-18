using Common;
using Common.WebAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RF_Visitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string URL = "http://test.api.visitor.rfmember.net/api/building/open_door_qrcode?content={0}&item_id={1}&building_id={2}&type={3}";
        /// <summary>
        /// 门禁二维码验证地址: test.api.visitor.rfmember.net/api/building/open_door_qrcode?content=二维码内容&item_id=门禁ID&building_id=大厦ID&type=类型(1:进入, 2:出去)
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigPublic.Init();

            AutoRun();
        }

        private static void AutoRun()
        {
            var appName = System.Windows.Forms.Application.ProductName;
            var executePath = System.Windows.Forms.Application.ExecutablePath;
            Funs.runWhenStart(true, appName, executePath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var qrCode = "kdfhds";
            string error = "";
            Stopwatch sw = Stopwatch.StartNew();
            var json = CallApi(qrCode, ref error);
            sw.Stop();
            Console.WriteLine("Elapsed millseconds:" + sw.ElapsedMilliseconds);
            if (!error.IsEmpty())
            {
                lblResult.Content = error;
                return;
            }

            JavaScriptSerializer serialize = new JavaScriptSerializer();
            var result = serialize.Deserialize<RFJsonResult>(json);
            if (result.content && result.success && result.hasError == false)
            {
                lblResult.Content = "请通行";
            }
            else
            {
                lblResult.Content = "授权失败，禁止通行";
            }
        }

        private string CallApi(string qrcode, ref string error)
        {
            var url = string.Format(URL,
              qrcode,
              ConfigPublic.TermID,
              ConfigPublic.BuildingID,
              ConfigPublic.TypeID);

            var json = "";
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
                error = "API地址服务失败";
            }
            return json;
        }
    }
}
