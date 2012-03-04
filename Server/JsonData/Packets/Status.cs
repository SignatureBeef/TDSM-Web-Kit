using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebKit.Server.Stats;
using Terraria_Server.Networking;
using Terraria_Server;

namespace WebKit.Server.JsonData.Packets
{
    public struct Status : IPacket
    {
		public Dictionary<String, Object> Data { get; set; }

		public PacketId GetPacket()
        {
            return PacketId.stats;
        }

        public void Process(object[] args)
        {
            WebKit WebKit = (WebKit)args[0];

            var process = System.Diagnostics.Process.GetCurrentProcess();
            var time = process.TotalProcessorTime;

            Data["status"] = WebKit.ServerStatus;
            Data["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Data["users"] = Terraria_Server.Networking.ClientConnection.All.Count;
			Data["maxusers"] = SlotManager.MaxSlots;
			Data["userlist"] = Json.GetUserList();
			Data["ready"] = NetPlay.ServerUp;

            Data["cpu"] = String.Format("{0:0.00}% ({1})",
                SystemStats.GetCPUUsage(), time);

            Data["virmem"] = String.Format("{0:0.0}/{1:0.0}MB",
                process.VirtualMemorySize64 / 1024.0 / 1024.0,
                process.PeakVirtualMemorySize64 / 1024.0 / 1024.0);

            Data["phymem"] = String.Format("{0:0.0}/{1:0.0}MB",
                SystemStats.GetMemoryUsage() / 1024.0 / 1024.0,
                process.PeakWorkingSet64 / 1024.0 / 1024.0);
        }
    }
}

