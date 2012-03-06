// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;

namespace WebKit.Server.JsonData.Packets
{
    public class Chat : SerializablePacket
    {
		public override PacketId GetPacket()
        {
            return PacketId.Chat;
        }

		public override void Process(Args args)
        {
			if (args.Count == 0 || args[0].ToString() == "undefined")
			{
				Data["messages"] = args.WebKit.UserChat.Values;
				Data["timesent"] = DateTime.Now.ToBinary().ToString();
				return;
			}

			string timeStamp = args[0].ToString();
            if (timeStamp.Trim().Length == 0)
            {
                timeStamp = (-long.MaxValue).ToString();
            }

			var chatList = Json.GetUserChat(timeStamp, args.WebKit);
            if (chatList != null)
            {
                Data["messages"] = chatList;
				Data["timesent"] = DateTime.Now.ToBinary().ToString();
            }
        }
    }
}
