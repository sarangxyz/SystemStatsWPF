using System;
using System.Diagnostics;

namespace livechart2
{
    internal class RAMUsage
    {
        private PerformanceCounter cpuCounter = new PerformanceCounter();
        public double Memory { get; private set; }

        public Int64 UsedBytes { get; private set; }
        public Int64 TotalBytes { get; private set; }

        public RAMUsage()
        {
            Update();
        }

        public void Update()
        {
            Int64 phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
            TotalBytes = PerformanceInfo.GetTotalMemoryInMiB();
            UsedBytes = TotalBytes - phav;

            double percentFree = (double)((decimal)phav / (decimal)TotalBytes) * 100.0;
            double percentOccupied = 100.0 - percentFree;
            Memory = percentOccupied;
        }
    }
}
