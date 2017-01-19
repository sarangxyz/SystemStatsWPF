using System;


namespace livechart2
{
    internal class CPUUsage
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
}
