using Common;
using Common.WebAPI;
using RF_Visitor.Core;
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
    public partial class MainWindow
    {
        Core.Core vm = null;
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            vm = new Core.Core();
            this.DataContext = vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AutoRun();
            vm.Init();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vm.Dispose();
        }

        private static void AutoRun()
        {
            var appName = System.Windows.Forms.Application.ProductName;
            var executePath = System.Windows.Forms.Application.ExecutablePath;
            Funs.runWhenStart(false, appName, executePath);
        }

        private void btnIn_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                vm.QRReaderCallback_In("eyJ0IjoxLCJpZCI6NDUsImtleSI6ImQ5ZTg3YzkzLTU0OTUtNDNjZC05ZTM1LTM5NDhmODhkODBlYiJ9");
            });
        }

        private void btnOut_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                vm.QRReaderCallback_Out("456");
            });
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
