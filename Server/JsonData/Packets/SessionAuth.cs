using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebKit.Server.Auth;

namespace WebKit.Server.JsonData.Packets
{
    public class SessionAuth : IPacket
    {
        public string GetPacket()
        {
            return PacketId.SESSION_AUTH;
        }

        public Dictionary<String, Object> Process(object[] Data)
        {
            Dictionary<String, Object> array = new Dictionary<String, Object>();

            WebKit WebKit = (WebKit)Data[0];

            string user, ipAddress = Data[1].ToString();
            if (WebServer.ConnectList.TryGetValue(ipAddress, out user))
            {
                array["auth"] = Authentication.InSession(ipAddress, WebKit);
            }
            else
            {
                array["auth"] = false;
            }

            return array;
        }
    }
}
