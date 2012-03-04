using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData.Packets
{
    public struct Chat : IPacket
    {
		public Dictionary<String, Object> Data { get; set; }

		public PacketId GetPacket()
        {
            return PacketId.chat;
        }

		public void Process(object[] args)
        {
			WebKit WebKit = (WebKit)args[0];

			string timeStamp = args[2].ToString();
            if (timeStamp.Trim().Length == 0)
            {
                timeStamp = (-long.MaxValue).ToString();
            }

            var chatList = Json.GetUserChat(timeStamp, WebKit);
            if (chatList != null)
            {
                Data["messages"] = chatList;
				Data["timesent"] = DateTime.Now.ToBinary().ToString();
            }
        }
    }
}
