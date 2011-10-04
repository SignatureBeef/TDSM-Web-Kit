using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData.Packets
{
    public struct Chat : IPacket
    {
        public string GetPacket()
        {
            return PacketId.CHAT;
        }

        public Dictionary<String, Object> Process(object[] Data)
        {
            Dictionary<String, Object> array = new Dictionary<String, Object>();

            WebKit WebKit = (WebKit)Data[0];

            string timeStamp = Data[2].ToString();
            if (timeStamp.Trim().Length == 0)
            {
                timeStamp = (-long.MaxValue).ToString();
            }

            List<WebMessage> chatList = Json.GetUserChat(timeStamp, WebKit);

            if (chatList != null)
            {
                array["messages"] = chatList;
                array["timesent"] = DateTime.Now.ToBinary().ToString();
            }

            return array;
        }
    }
}
