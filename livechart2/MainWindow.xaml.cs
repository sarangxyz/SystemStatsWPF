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

    public class CPUUsage
    {
        //private PerformanceCounter cpuCounter = new PerformanceCounter();
        public double CPU { get; private set; }

        public CPUUsage()
        {
            //cpuCounter.CategoryName = "Processor";
            //cpuCounter.CounterName = "% Processor Time";
            //cpuCounter.InstanceName = "_Total";

            Update();
        }

        private static long ToLong(System.Runtime.InteropServices.ComTypes.FILETIME fileTime)
        {
            long returnedLong;
            // Convert 4 high-order bytes to a byte array
            byte[] highBytes = BitConverter.GetBytes(fileTime.dwHighDateTime);
            // Resize the array to 8 bytes (for a Long)
            Array.Resize(ref highBytes, 8);

            // Assign high-order bytes to first 4 bytes of Long
            returnedLong = BitConverter.ToInt64(highBytes, 0);
            // Shift high-order bytes into position
            returnedLong = returnedLong << 32;
            // Or with low-order bytes
            returnedLong = returnedLong | fileTime.dwLowDateTime;
            // Return long 
            return returnedLong;
        }

        public void Update()
        {
            System.Runtime.InteropServices.ComTypes.FILETIME idleTime1, krnlTime1, userTime1;
            
            PerformanceInfo.GetSystemTimes(out idleTime1, out krnlTime1, out userTime1);

            System.Threading.Thread.Sleep(100);

            System.Runtime.InteropServices.ComTypes.FILETIME idleTime2, krnlTime2, userTime2;
            PerformanceInfo.GetSystemTimes(out idleTime2, out krnlTime2, out userTime2);

            long idleTime = ToLong(idleTime2) - ToLong(idleTime1);
            long krnlTime = ToLong(krnlTime2) - ToLong(krnlTime1);
            long userTime = ToLong(userTime2) - ToLong(userTime1);
            long total = krnlTime + userTime;

            CPU = (double)(total - idleTime) / total * 100.0;
        }
    }

    internal static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemTimes(out System.Runtime.InteropServices.ComTypes.FILETIME lpIdleTime, 
                                          out System.Runtime.InteropServices.ComTypes.FILETIME lpKernelTime, 
                                          out System.Runtime.InteropServices.ComTypes.FILETIME lpUserTime);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }
    }

    public class RAMUsage
    {
        private PerformanceCounter cpuCounter = new PerformanceCounter();
        public double Memory { get; private set; }

        public RAMUsage()
        {
            Update();
        }

        public void Update()
        {
            Int64 phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
            Int64 tot = PerformanceInfo.GetTotalMemoryInMiB();
            double percentFree = (double)((decimal)phav / (decimal)tot) * 100.0;
            double percentOccupied = 100.0 - percentFree;
            Memory = percentOccupied;
        }
    }

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

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _timer.Stop();
            _timer.Dispose();
        }

        private static Action EmptyDelegate = delegate () { };
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CPU.Update();
            RAM.Update();
            this.Dispatcher.Invoke(() =>
            {
                _cpuDial.Value = CPU.CPU;
                _ramDial.Value = RAM.Memory;
            });            
        }
    }
}
