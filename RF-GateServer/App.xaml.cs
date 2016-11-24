using System;
using System.Threading;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Common.Dialog;
using RF_GateServer.Core;

namespace RF_GateServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var create = false;
            var metux = new Mutex(true, "rf-server", out create);
            if (create)
            {
                ConfigProfile.ReadConfig();
                MainWindow window = new RF_GateServer.MainWindow();
                Application.Current.MainWindow = window;
                Application.Current.MainWindow.ShowDialog();
            }
            else
            {
                CustomDialog.Show("系统已运行！");
                Application.Current.Shutdown();
            }
        }
    }
}
