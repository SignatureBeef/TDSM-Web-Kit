using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Web.Script.Serialization;
using Terraria_Server;
using Terraria_Server.Networking;
using WebKit.Server.Auth;
using System.Net;
using WebKit.Server.Stats;
using System.Diagnostics;
using System.IO;

namespace WebKit.Server.JsonData
{
    public class Json
    {
        public const string JSON_SEPERATOR = "/data.json?request=";        

        public static bool ProcessJsonHeader(WebKit WebKit, HttpListenerContext context)
        {
            string RequestURL = context.Request.RawUrl;
            if (RequestURL.StartsWith(JSON_SEPERATOR))
            {
                string data = RequestURL.Remove(0, RequestURL.IndexOf(JSON_SEPERATOR) + JSON_SEPERATOR.Length).Replace("%20", " ");

                List<String> keys = data.Split('&').ToList<String>();

                Parser.ParseAndProcess(WebKit, context, keys);

                return true;
            }
            return false;
        }

        public static string Serialize(JavaScriptSerializer serializer, Dictionary<String, Object> data)
        {
            return serializer.Serialize(data);
        }

        public static List<String> GetUserList()
        {
            List<String> ret = new List<String>();
            foreach (Player player in Main.players)
            {
                if(player.Name != null)
                    ret.Add(player.Name);
            }
            return ret;
        }

        public static List<WebMessage> GetUserChat(string TimeStamp, WebKit WebKit)
        {
            List<WebMessage> ret = new List<WebMessage>();
            Dictionary<String, WebMessage> data = WebKit.UserChat;
            long timestamp;
            if (long.TryParse(TimeStamp, out timestamp))
            {
                foreach (WebMessage chatMsg in data.Values
                    .Where(X => X.timesent.ToBinary() > timestamp))
                {
                    long t = chatMsg.timesent.ToBinary();
                    ret.Add(chatMsg);
                }
            }

            return ret;
        }
    }
}
