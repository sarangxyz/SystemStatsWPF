using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace livechart2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _window = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.Exit += App_Exit;


            // Create main application window, starting minimized if specified
            _window = new MainWindow();
            _window.Show();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            _window.StopTimerOnShutdown();
        }
    }
}
