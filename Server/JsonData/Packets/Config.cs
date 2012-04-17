// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;

namespace WebKit.Server.JsonData.Packets
{
    public class Config : SerializablePacket
	{
		public override PacketId GetPacket()
        {
            return PacketId.Config;
        }

		public override void Process(Args args)
        {
			var webKit = args.WebKit;
			Data["maxLines"] = webKit.Properties.MaxChatLines;
			Data["main-interval"] = webKit.MainUpdateInterval;
        }
    }
}