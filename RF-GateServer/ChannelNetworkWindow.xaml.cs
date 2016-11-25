using RF_GateServer.DataManager;
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
    /// ChannelNetworkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChannelNetworkWindow : Window
    {
        private int pageIndex = 1;
        private int pageSize = 30;
        private int totalCount = 0;
        public ChannelNetworkWindow()
        {
            InitializeComponent();
        }

        private void btnSearch_click(object sender, RoutedEventArgs e)
        {
            var query = SQLite.Current.Query(txtIp.Text, pageIndex, pageSize, out totalCount);
            dgHistory.ItemsSource = query;
        }
    }
}
