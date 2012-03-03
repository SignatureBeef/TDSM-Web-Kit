using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData.Packets
{
    public struct Config : IPacket
    {
        public string GetPacket()
        {
            return PacketId.CONFIG;
        }

        public Dictionary<String, Object> Process(object[] Data)
        {
            WebKit WebKit = (WebKit)Data[0];

            Dictionary<String, Object> array = new Dictionary<String, Object>();

            array["maxLines"] = WebKit.Properties.MaxChatLines;
			array["main-interval"] = WebKit.Properties.UpdateInterval;

            return array;
        }
    }
}