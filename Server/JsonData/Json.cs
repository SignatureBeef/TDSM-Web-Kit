// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Terraria_Server;
using System.Net;
using WebKit.Server.Misc;

namespace WebKit.Server.JsonData
{
	public static class Json
	{
		public const string JSON_SEPERATOR = "/data.json?request=";        

		public static bool ProcessJsonHeader(WebKit webKit, HttpListenerContext context, string user, string ipAddress)
		{
			string requestUrl = context.Request.RawUrl;
			if (requestUrl.StartsWith(JSON_SEPERATOR))
			{
				string data = requestUrl.Remove(0, requestUrl.IndexOf(JSON_SEPERATOR) + JSON_SEPERATOR.Length).Replace("%20", " ");

				var args = data.Split('&');

				Parser.ParseAndProcess(webKit, context, args, user, ipAddress);

				return true;
			}
			return false;
		}

		public static string Serialize(JavaScriptSerializer serializer, object data)
		{
			return serializer.Serialize(data);
		}

		public static string Serialize(WebServer server, object data)
		{
			return server.Serializer.Serialize(data);
		}

		public static List<String> GetUserList()
		{
			List<String> ret = new List<String>();
			foreach (Player player in Main.players)
			{
				if (player.Name != null)
					ret.Add(player.Name);
			}
			return ret;
		}

		public static List<WebMessage> GetUserChat(string timeStamp, WebKit webkit)
		{
			List<WebMessage> ret = new List<WebMessage>();
			MultiArray<String, WebMessage> data = webkit.UserChat.FieldwiseClone();
			long timestamp;
			if (long.TryParse(timeStamp, out timestamp))
			{
				foreach (WebMessage chatMsg in data.Values.Where(x => x.TimeSent.ToBinary() > timestamp))
				{
					ret.Add(chatMsg);
				}
			}

			return ret.SortByDescending();
		}
	}

	public static class ListExtensions
	{
		public static List<WebMessage> SortByDescending(this List<WebMessage> list)
		{
			var ret = list.FieldwiseClone();
			ret.Sort(CompareTime);
			return ret;
		}

		private static int CompareTime(WebMessage msg1, WebMessage msg2)
		{
			return msg1.TimeSent.CompareTo(msg2.TimeSent);
		}
	}
}