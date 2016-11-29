using RF_GateServer.Core;
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
    /// InOutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InOutWindow : Window
    {
        private int pageIndex = 1;
        private int pageSize = 30;
        private int totalPageCount = 0;

        public InOutWindow()
        {
            InitializeComponent();

            this.Loaded += InOutWindow_Loaded;
        }

        private void InOutWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cmbChannels.Items.Insert(0, "全部");
            foreach (var channel in ComServerController.Current.Channels)
            {
                cmbChannels.Items.Add(channel.Name);
            }
            cmbChannels.SelectedIndex = 0;

            dtStart.Value = DateTime.Now;
            dtEnd.Value = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

            lbltotal.Content = "0";
            lblpage.Content = "0/0";
        }

        private void btnSearch_click(object sender, RoutedEventArgs e)
        {
            Query();
        }

        private void Query()
        {
            var totalCount = 0;
            PageQuery page = new PageQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var channel = cmbChannels.SelectedItem.ToString();
            if (channel == "全部")
                channel = "";

            var query = SQLite.Current.QueryInOut(channel, dtStart.Value, dtEnd.Value, page);
            dgHistory.ItemsSource = query;

            lbltotal.Content = page.TotalCount.ToString();
            totalPageCount = totalCount / pageSize;
            if (totalCount % pageSize != 0)
                totalPageCount++;
            lblpage.Content = string.Format("{0}/{1}", pageIndex, totalPageCount);
        }

        private void btnPre_click(object sender, RoutedEventArgs e)
        {
            pageIndex--;
            if (pageIndex < 1)
                pageIndex = 1;
            Query();
        }

        private void btnNext_click(object sender, RoutedEventArgs e)
        {
            pageIndex++;
            if (pageIndex > totalPageCount)
                pageIndex = totalPageCount;
            Query();
        }
    }
}
