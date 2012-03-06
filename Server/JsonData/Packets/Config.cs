using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData.Packets
{
    public struct Config : IPacket
	{
		public Dictionary<String, Object> Data { get; set; }

		public PacketId GetPacket()
        {
            return PacketId.config;
        }

		public void Process(Args args)
        {
			var webKit = args.WebKit;
			Data["maxLines"] = webKit.Properties.MaxChatLines;
			Data["main-interval"] = webKit.MainUpdateInterval;
        }
    }
}