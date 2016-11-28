﻿using RF_GateServer.DataManager;
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
    /// 通道异常历史记录
    /// </summary>
    public partial class ChannelNetworkWindow
    {
        private int pageIndex = 1;
        private int pageSize = 30;
        private int totalPageCount = 0;

        public ChannelNetworkWindow()
        {
            InitializeComponent();
            this.Loaded += ChannelNetworkWindow_Loaded;
        }

        private void ChannelNetworkWindow_Loaded(object sender, RoutedEventArgs e)
        {
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

            var query = SQLite.Current.QueryState(txtIp.Text, page);
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

        private void btnSetting_click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ok");
        }
    }
}
