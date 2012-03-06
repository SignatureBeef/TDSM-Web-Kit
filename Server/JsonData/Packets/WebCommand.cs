using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using Terraria_Server.Logging;

namespace WebKit.Server.JsonData.Packets
{
    public struct WebCommand : IPacket
	{
		public Dictionary<String, Object> Data { get; set; }

		public PacketId GetPacket()
        {
            return PacketId.webcommand;
        }

        public void Process(Args args)
        {
			string Command = args[0].ToString();

			ProgramLog.Log("Web command `{0}` from {1}", Command, args.IpAddress);
			Program.commandParser.ParseAndProcess(args.WebKit.WebSender, Command);
        }
    }
}
