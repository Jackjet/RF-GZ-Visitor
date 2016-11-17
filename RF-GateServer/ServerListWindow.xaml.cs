using RF_GateServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RF_GateServer
{
    /// <summary>
    /// ServerListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ServerListWindow : Window
    {
        public ServerListWindow()
        {
            InitializeComponent();
            this.Loaded += ServerListWindow_Loaded;
        }

        private void ServerListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgChannel.ItemsSource = MapReader.Read();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ServerConfigWindow config = new ServerConfigWindow(new Channel());
            config.ShowDialog();
            MapReader.Save(config.Channel);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
