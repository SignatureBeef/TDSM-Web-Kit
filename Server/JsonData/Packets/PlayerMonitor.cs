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
        public string GetPacket()
        {
            return PacketId.PLAYER_MONITOR;
        }

        public Dictionary<String, Object> Process(object[] Data)
        {
            Dictionary<String, Object> array = new Dictionary<String, Object>();

            WebKit WebKit = (WebKit)Data[0];

            var Player = Terraria_Server.Server.GetPlayerByName(Data[2].ToString());

            if (Player == null)
                array["data"] = "Player is no longer online.";
            else
                array["data"] = UserMoniter.SerializePlayer(Player);

            return array;
        }
    }
}
