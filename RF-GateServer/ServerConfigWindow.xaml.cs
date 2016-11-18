using Common;
using Common.Dialog;
using RF_GateServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            if (channel.Index.IsEmpty())
            {
                CustomDialog.Show("请输入编号！");
                return;
            }
            else
            {
                if (CheckIndexExists(channel.Index))
                {
                    CustomDialog.Show("编号已存在！");
                    return;
                }
            }
            if (channel.Area.IsEmpty())
            {
                CustomDialog.Show("请输入区域名称！");
                return;
            }
            if (channel.Name.IsEmpty())
            {
                CustomDialog.Show("请输入通道名称！");
                return;
            }
            else
            {
                if (CheckNameExists(channel.Name))
                {
                    CustomDialog.Show("通道名称已存在！");
                    return;
                }
            }
            if (channel.InIp.IsEmpty() && channel.OutIp.IsEmpty())
            {
                CustomDialog.Show("至少输入一个IP地址！");
                return;
            }
            if (!channel.InIp.IsEmpty())
            {
                if (!channel.InIp.IsIPAddress())
                {
                    CustomDialog.Show("请输入正确的IP地址格式！");
                    return;
                }

                if (CheckIPExists(channel.InIp))
                {
                    CustomDialog.Show("通道IP地址已存在！");
                    return;
                }
            }
            if (!channel.OutIp.IsEmpty())
            {
                if (!channel.OutIp.IsIPAddress())
                {
                    CustomDialog.Show("请输入正确的IP地址格式！");
                    return;
                }
                if (CheckIPExists(channel.OutIp))
                {
                    CustomDialog.Show("通道IP地址已存在！");
                    return;
                }
            }
            if (channel.GateIp.IsEmpty())
            {
                CustomDialog.Show("请输入网络继电器IP地址！");
                return;
            }
            else
            {
                if (!channel.GateIp.IsIPAddress())
                {
                    CustomDialog.Show("请输入正确的IP地址格式！");
                    return;
                }
            }
            this.DialogResult = true;
        }

        private bool CheckIndexExists(string index)
        {
            var channel = ComServerController.Instance.Channels.FirstOrDefault(s => s.Index == index);
            if (channel != null)
                return true;
            else
                return false;
        }

        private bool CheckNameExists(string name)
        {
            var channel = ComServerController.Instance.Channels.FirstOrDefault(s => s.Name == name);
            if (channel != null)
                return true;
            else
                return false;
        }

        private bool CheckIPExists(string ip)
        {
            var channel = ComServerController.Instance.Channels.FirstOrDefault(s => s.InIp == ip || s.OutIp == ip);
            if (channel != null)
                return true;
            else
                return false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
