using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData.Packets
{
    public struct WebChat : IPacket
    {
        public string GetPacket()
        {
            return PacketId.WEB_CHAT;
        }

        public Dictionary<String, Object> Process(object[] Data)
        {
            Dictionary<String, Object> array = new Dictionary<String, Object>();
            string msg = Data[2].ToString().Trim();

            if (msg.Contains("="))
            {
                Terraria_Server.Server.notifyAll("Web: " + msg);
            }
            
            return array;
        }
    }
}

