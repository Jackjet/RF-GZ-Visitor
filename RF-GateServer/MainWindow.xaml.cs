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
using System.ComponentModel;
using RF_GateServer.DataManager;

namespace RF_GateServer
{
    /// <summary>
    /// 网络串口监控服务器
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string exit_tip = "确认退出系统吗？";
        private const string stopService_tip = "确定停止服务吗？";
        private const string restartService_tip = "确定启动服务吗？";
        private const string deleteChannel_tip = "确认删除选中的通道吗？";

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var confirm = CustomDialog.Confirm(exit_tip);
            if (confirm == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
            ComServerController.Current.Stop();
            base.OnClosing(e);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite.Current.Init();
            ComServerController.Current.Run();
            this.DataContext = ComServerController.Current;
        }

        private void btnInOut_click(object sender, RoutedEventArgs e)
        {
            InOutWindow inout = new InOutWindow();
            inout.ShowDialog();
        }

        private void btnConnectReader_click(object sender, RoutedEventArgs e)
        {
            ChannelNetworkWindow window = new ChannelNetworkWindow();
            window.ShowDialog();
        }

        private void btnAddChannel_click(object sender, RoutedEventArgs e)
        {
            ServerConfigWindow config = new ServerConfigWindow(new Channel(), true);
            var dialog = config.ShowDialog().Value;
            if (dialog)
            {
                ComServerController.Current.Channels.Add(config.Channel);
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
            ComServerController.Current.RemoveChannel(channel);
        }

        private void dgChannel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgChannel.SelectedItem == null)
            {
                return;
            }
            var channel = (Channel)dgChannel.SelectedItem;
            ServerConfigWindow config = new ServerConfigWindow(channel, false);
            var dialog = config.ShowDialog().Value;
            if (dialog)
            {

            }
        }

        private void btnStopServer_click(object sender, RoutedEventArgs e)
        {
            if (ComServerController.Current.IsRunning)
            {
                //运行状态
                if (CustomDialog.Confirm(stopService_tip) == MessageBoxResult.No)
                {
                    return;
                }
                ComServerController.Current.Stop();
                miService.Header = "启动服务";
            }
            else
            {
                //停止状态
                if (CustomDialog.Confirm(restartService_tip) == MessageBoxResult.No)
                {
                    return;
                }
                ComServerController.Current.Run();
                miService.Header = "停止服务";
            }
        }

        private void btnExit_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
