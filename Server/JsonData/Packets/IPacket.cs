using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData.Packets
{
    public interface IPacket
    {
        string GetPacket();

        Dictionary<String, Object> Process(object[] Data);
    }
}
