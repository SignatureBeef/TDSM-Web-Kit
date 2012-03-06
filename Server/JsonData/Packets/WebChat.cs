// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using Terraria_Server.Logging;

namespace WebKit.Server.JsonData.Packets
{
	public class WebChat : SerializablePacket
	{
		public override PacketId GetPacket()
		{
			return PacketId.WebChat;
		}

		public override void Process(Args args)
		{
			var message = args[0].ToString().Trim();

			//if (msg.Contains("="))

			var logger = (SendingLogger)SendingLoggerExtension.WEB;
			Terraria_Server.Server.notifyAll("Web: " + message, true, logger);
		}
	}

	public enum SendingLoggerExtension
	{
		WEB = 3
	}
}

