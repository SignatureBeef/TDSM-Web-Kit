using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebKit.Server.Auth;

namespace WebKit.Server.JsonData.Packets
{
    public struct Credentials : IPacket
    {
        public string GetPacket()
        {
            return PacketId.CREDENTIALS;
        }

        public Dictionary<String, Object> Process(object[] Data)
        {
            Dictionary<String, Object> array = new Dictionary<String, Object>();

            WebKit WebKit = (WebKit)Data[0];
            string user = Data[2].ToString().Trim();
            string pass = Data[3].ToString().Trim();
            string ipAddress = Data[1].ToString().Trim();

            AuthStatus verificationStatus = Authentication.isCredentialsTheSame(user, pass, WebKit);

            array["match"] = verificationStatus;

            if (verificationStatus == AuthStatus.MATCH)
            {
                DateTime date = DateTime.Now;
                date = date.AddMinutes(WebKit.WebServer.SessionTime);

                if (WebKit.WebSessions.Keys.Contains(ipAddress))
                {
                    WebKit.WebSessions.Remove(ipAddress);
                }

                WebKit.WebSessions.Add(ipAddress, date);
            }

            if (WebServer.ConnectList.Keys.Contains(ipAddress))
            {
                WebServer.ConnectList.Remove(ipAddress);
            }
            WebServer.ConnectList.Add(ipAddress, user);

            return array;
        }
    }
}
