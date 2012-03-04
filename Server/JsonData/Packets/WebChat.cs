using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData.Packets
{
    public struct WebChat : IPacket
    {
		public Dictionary<String, Object> _data;
		public Dictionary<String, Object> Data { get { return _data; } set { return; } }

		public PacketId GetPacket()
        {
            return PacketId.webchat;
        }

		public void Process(object[] args)
        {
			string msg = args[2].ToString().Trim();

            if (msg.Contains("="))
				Terraria_Server.Server.notifyAll("Web: " + msg);
        }
    }
}

