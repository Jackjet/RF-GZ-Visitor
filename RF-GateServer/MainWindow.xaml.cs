using KonNaDSwitch;
using RF_GateServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RF_GateServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SortedList<string, IQRReader> readerList = new SortedList<string, IQRReader>();
        private SortedDictionary<string, IGate> switchList = new SortedDictionary<string, IGate>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnectSwitch_Click(object sender, RoutedEventArgs e)
        {
            ConnectIPSwitch();
        }

        private void ConnectIPSwitch()
        {
            for (int i = 100; i <= 164; i++)
            {
                var ip = "192.168.1." + i;
                var ipGate = new IPGate();
                //ipGate.Connect(ip);
                switchList.Add(ip, ipGate);
            }
        }

        private void btnConnectReader_Click(object sender, RoutedEventArgs e)
        {
            ConnectReader();
        }

        private void ConnectReader()
        {
            for (int i = 200; i < 255; i++)
            {
                var ip = "192.168.1." + i;
                var reader = new WeiGuangQRReader();
                var connect = reader.Connect(ip, 9877);
                if (connect)
                {
                    readerList.Add(ip, reader);
                    reader.BeginRead(ReaderCallback);
                }
            }
        }

        private void ReaderCallback(string qrcode)
        {
            //Console.Out.WriteLine("qrcode->" + qrcode);
            //var ip = "";
            //var ipSwitch = switchList[ip];
            //ipSwitch.OpenIn(1000);
        }

        private void ReaderCallback(string ip, string qrcode)
        {
            Console.Out.WriteLine(Thread.CurrentThread.ManagedThreadId + " " + ip + " " + qrcode);
        }

        bool stop = false;
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var i = 1;
            Task.Factory.StartNew(() =>
            {
                while (!stop)
                {
                    Action<string, string> act1 = ReaderCallback;
                    act1.BeginInvoke(i.ToString(), i.ToString(), null, null);
                    //act1.Invoke(i.ToString(), i.ToString());
                    i++;
                    Thread.Sleep(1);
                }
            });
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            stop = true;
        }
    }
}
