using Common;
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
    /// ServerConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ServerConfigWindow : Window
    {
        private Channel channel = null;
        public ServerConfigWindow(Channel channel)
        {
            InitializeComponent();
            this.channel = channel;
            this.DataContext = channel;
        }

        public Channel Channel
        {
            get
            {
                return (Channel)this.DataContext;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
