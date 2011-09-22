using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebKit.Server.Stats;
using Terraria_Server.Networking;

namespace WebKit.Server.JsonData.Packets
{
    public class Status : IPacket
    {
        public string GetPacket()
        {
            return PacketId.STATUS;
        }

        public Dictionary<String, Object> Process(object[] Data)
        {
            Dictionary<String, Object> array = new Dictionary<String, Object>();

            WebKit WebKit = (WebKit)Data[0];

            var process = System.Diagnostics.Process.GetCurrentProcess();
            var time = process.TotalProcessorTime;

            array["status"] = WebKit.ServerStatus;
            array["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            array["users"] = Terraria_Server.Networking.ClientConnection.All.Count;
            array["maxusers"] = SlotManager.MaxSlots;
            array["userlist"] = Json.GetUserList();

            array["cpu"] = String.Format("{0:0.00}% ({1})",
                SystemStats.GetCPUUsage(), time);

            array["virmem"] = String.Format("{0:0.0}/{1:0.0}MB",
                process.VirtualMemorySize64 / 1024.0 / 1024.0,
                process.PeakVirtualMemorySize64 / 1024.0 / 1024.0);

            array["phymem"] = String.Format("{0:0.0}/{1:0.0}MB",
                SystemStats.GetMemoryUsage() / 1024.0 / 1024.0,
                process.PeakWorkingSet64 / 1024.0 / 1024.0);

            return array;
        }
    }
}

