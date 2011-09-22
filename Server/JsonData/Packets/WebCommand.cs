using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData.Packets
{
    public class WebCommand : IPacket
    {
        public string GetPacket()
        {
            return PacketId.WEB_COMMAND;
        }

        public Dictionary<String, Object> Process(object[] Data)
        {
            Dictionary<String, Object> array = new Dictionary<String, Object>();


            return array;
        }
    }
}
