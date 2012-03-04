using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace WebKit.Server.Stats
{
    public class SystemStats
    {
        public static PerformanceCounter memCounter =
            new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
        public static PerformanceCounter cpuCounter =
            new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);


        public static float GetMemoryUsage()
        {
            return memCounter.NextValue();
        }

        public static float GetCPUUsage()
        {
			return cpuCounter.NextValue();
        }
    }
}
