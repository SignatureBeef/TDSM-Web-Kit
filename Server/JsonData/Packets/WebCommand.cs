// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using Terraria_Server;
using Terraria_Server.Logging;

namespace WebKit.Server.JsonData.Packets
{
	public class WebCommand : SerializablePacket
	{
		public override PacketId GetPacket()
        {
            return PacketId.WebCommand;
        }

        public override void Process(Args args)
        {
			string command = args[0].ToString();

			ProgramLog.Log("Web command `{0}` from {1}", command, args.IpAddress);
			Program.commandParser.ParseAndProcess(args.WebKit.WebSender, command);
        }
    }
}
