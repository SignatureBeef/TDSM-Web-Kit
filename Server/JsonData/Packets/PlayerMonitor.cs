using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebKit.Server.Stats;
using Terraria_Server;

namespace WebKit.Server.JsonData.Packets
{
    public struct PlayerMonitor : IPacket
    {
		public Dictionary<String, Object> Data { get; set; }

		public PacketId GetPacket()
        {
            return PacketId.pdata;
        }

        public void Process(Args args)
        {
			var Player = Terraria_Server.Server.GetPlayerByName(args[0].ToString());

            if (Player == null)
                Data["data"] = "Player is no longer online.";
            else
                Data["data"] = UserMoniter.SerializePlayer(Player);
        }
    }
}
