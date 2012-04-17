// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Diagnostics;

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

        public static float GetCpuUsage()
        {
			return cpuCounter.NextValue();
        }
    }
}
