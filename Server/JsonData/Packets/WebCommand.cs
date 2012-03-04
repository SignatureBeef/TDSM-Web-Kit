using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;

namespace WebKit.Server.JsonData.Packets
{
    public struct WebCommand : IPacket
	{
		public Dictionary<String, Object> Data { get; set; }

		public PacketId GetPacket()
        {
            return PacketId.webcommand;
        }

        public void Process(object[] args)
        {
            WebKit WebKit = (WebKit)args[0];
            string Command = args[2].ToString();

            Program.commandParser.ParseAndProcess(WebKit.WebSender, Command);
        }
    }
}
