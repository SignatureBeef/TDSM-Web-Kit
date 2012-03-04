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

		public void Process(object[] args)
        {
            WebKit WebKit = (WebKit)args[0];
			
            Data["maxLines"] = WebKit.Properties.MaxChatLines;
			Data["main-interval"] = WebKit.MainUpdateInterval;
        }
    }
}