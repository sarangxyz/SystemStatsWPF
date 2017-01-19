using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
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
using System.Runtime.InteropServices;

using FILETIME = System.Runtime.InteropServices.FILETIME;
using System.Windows.Threading;

namespace livechart2
{

    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CPUUsage CPU = new CPUUsage();
        private RAMUsage RAM = new RAMUsage();
        private Timer _timer = new Timer();

        public MainWindow()
        {
            InitializeComponent();

            _timer.Enabled = true;
            _timer.Interval = 250;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        internal void StopTimerOnShutdown()
        {
            _timer.Enabled = false;
            _timer.Stop();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _timer.Stop();
            _timer.Dispose();
        }

        private static double Clamp(double val, double min, double max)
        {
            if (val < min)
                return min;
            else if (val > max)
                return max;
            return val;
        }

        private double ToGBFromMBytes(Int64 bytes)
        {
            return (double)bytes / 1024.0;
        }

        private static Action EmptyDelegate = delegate () { };
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (this)
            {
                CPU.Update();
                RAM.Update();
                this.Dispatcher.Invoke(() =>
                {
                    var cpuVal = Clamp(CPU.CPU, 0.0, 100.0);
                    var ramVal = Clamp(RAM.Memory, 0.0, 100.0);
                    _cpuDial.Value = cpuVal;
                    _ramDial.Value = ramVal;

                    _cpuLabel.Content = string.Format("CPU {0} %", cpuVal.ToString("N2"));
                    _ramLabel.Content = string.Format("RAM {0}/{1} GB", ToGBFromMBytes(RAM.UsedBytes).ToString("N2"), ToGBFromMBytes(RAM.TotalBytes).ToString("N2"));
                });
            }       
        }
    }
}
