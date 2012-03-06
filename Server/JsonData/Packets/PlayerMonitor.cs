// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using WebKit.Server.Stats;

namespace WebKit.Server.JsonData.Packets
{
    public class PlayerMonitor : SerializablePacket
    {
		public override PacketId GetPacket()
        {
            return PacketId.Pdata;
        }

		public override void Process(Args args)
        {
			var player = Terraria_Server.Server.GetPlayerByName(args[0].ToString());
            if (player == null)
                Data["data"] = "Player is no longer online.";
            else
                Data["data"] = UserMoniter.SerializePlayer(player);
        }
    }
}
