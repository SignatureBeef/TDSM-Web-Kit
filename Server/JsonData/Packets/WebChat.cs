using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Logging;

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

		public void Process(Args args)
		{
			//string msg = args[2].ToString().Trim();
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

