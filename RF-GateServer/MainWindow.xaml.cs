using RF_GateServer.Core;
using Common;
using RF_GateServer.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Common.Dialog;

namespace RF_GateServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string stopService_tip = "确定停止服务吗？";
        private const string deleteChannel_tip = "确认删除选中的通道吗？";
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ComServerController.Instance.Run();
            dgChannel.ItemsSource = ComServerController.Instance.Channels;
            dgLiving.ItemsSource = ComServerController.Instance.LivingDataCollection;
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                ServerListWindow server = new RF_GateServer.ServerListWindow();
                server.Show();
            }
        }

        private void btnConnectReader_Click(object sender, RoutedEventArgs e)
        {
            ConnectReader();
        }

        private void ConnectReader()
        {
            ComServerController.Instance.Channels[0].CheckIn("123");
            ComServerController.Instance.Channels[1].CheckOut("456");
        }

        private void btnStopServer_click(object sender, RoutedEventArgs e)
        {
            if (CustomDialog.Confirm(stopService_tip) == MessageBoxResult.No)
            {
                return;
            }
            ComServerController.Instance.Stop();
        }

        private void btnAddChannel_click(object sender, RoutedEventArgs e)
        {
            ServerConfigWindow config = new ServerConfigWindow(new Channel());
            var dialog = config.ShowDialog().Value;
            if (dialog)
            {
                ComServerController.Instance.Channels.Add(config.Channel);
                MapReader.Save(config.Channel);
            }
        }

        private void btnDeleteChannel_click(object sender, RoutedEventArgs e)
        {
            if (dgChannel.SelectedItem == null)
            {
                return;
            }

            var confirm = CustomDialog.Confirm(deleteChannel_tip);
            if (confirm == MessageBoxResult.No)
                return;

            Channel channel = (Channel)dgChannel.SelectedItem;
            MapReader.Delete(channel.Index);
            ComServerController.Instance.RemoveChannel(channel);
        }
    }
}
