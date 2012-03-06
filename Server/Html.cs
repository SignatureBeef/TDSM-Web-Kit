// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria_Server.Logging;
using WebKit.Server.JsonData;
using System.Net;
using WebKit.Server.Auth;

namespace WebKit.Server
{
	public struct TDSMBasicIdentity
	{
		public WebKit WebKit { get; set; }

		public string Name { get; set; }
		public string Password { get; set; }

		public AuthStatus AuthStatus
		{
			get
			{
				return Authentication.IsCredentialsTheSame(this.Name, this.Password, this.WebKit);
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return this.AuthStatus == AuthStatus.MATCH;
			}
		}
	}

	public struct Identity
	{
		public string IpAddress { get; set; }
		public DateTime LastUpdate { get; set; }
	}

	public static class Html
	{
		public const String AGENT = "TDSM WebKit";

		public static TDSMBasicIdentity ToTDSMIdentity(this HttpListenerBasicIdentity identity, WebKit webKit)
		{
			return new TDSMBasicIdentity()
			{
				WebKit = webKit,
				Name = identity.Name,
				Password = identity.Password
			};
		}

		public static bool NeedsKick(string ipAddress, string name, WebKit webKit, out int index)
		{
			index = default(Int32);
			lock (webKit.KickList)
			{
				var list = webKit.KickList;
				for (var i = 0; i < list.Count; i++)
				{
					var pair = list.ElementAt(i);
					if (pair.Key == name && pair.Value.IpAddress == ipAddress)
					{
						index = i;
						return true;
					}
				}
			}

			return false;
		}

		public static void RemoveKickedUser(string ipAddress, string name, WebKit webKit, int slot)
		{
			lock (webKit.KickList)
			{
				var list = webKit.KickList.FieldwiseClone();
				foreach (var pair in list)
				{
					if (pair.Key == name && pair.Value.IpAddress == ipAddress)
						webKit.KickList.RemoveAt(slot);
				}
			}
		}

		public static bool CheckAuthenticity(HttpListenerContext context, WebKit webkit, string httpData, string ipAddress)
		{
			var identity = context.User.Identity;

			int slot;
			if (NeedsKick(ipAddress, identity.Name, webkit, out slot))
			{
				RemoveKickedUser(ipAddress, identity.Name, webkit, slot);

				var res = new Dictionary<String, Object>();
				res["main-interval-rm"] = "http://tdsm.org";
				var serialized = Json.Serialize(webkit.WebServer, res);
				context.WriteString(serialized);

				WebKit.Log("{0} disconnected from {1}", identity.Name, ipAddress ?? "HTTP");
				return false;
			}

			switch (identity.AuthenticationType)
			{
				case "Basic":
					var basicIdentity = (identity as HttpListenerBasicIdentity).ToTDSMIdentity(webkit);

					lock (webkit.WebSessions)
					{
						if (basicIdentity.AuthStatus != AuthStatus.MATCH)
						{
							context.Disconnect("Credentials incorrect.");
							WebKit.Log("{0} disconnected from {1}", basicIdentity.Name, ipAddress ?? "HTTP");
							return false;
						}
						else
						{
							Identity ident;
							if (!webkit.WebSessions.ContainsKey(basicIdentity.Name))
								WebKit.Log("{0} logged in from {1}", basicIdentity.Name, ipAddress ?? "HTTP");
							else if (webkit.WebSessions.TryGetValue(basicIdentity.Name, out ident))
							{
								if ((DateTime.Now - ident.LastUpdate).TotalMilliseconds > (webkit.MainUpdateInterval * 2))
									WebKit.Log("{0} logged in from {1}", basicIdentity.Name, ipAddress ?? "HTTP");
							}
						}

						if (webkit.WebSessions.ContainsKey(basicIdentity.Name))
						{
							var newIdent = webkit.WebSessions[basicIdentity.Name];
							newIdent.IpAddress = ipAddress;
							newIdent.LastUpdate = DateTime.Now;
							webkit.WebSessions[basicIdentity.Name] = newIdent;
						}
						else
							webkit.WebSessions[basicIdentity.Name] = new Identity()
							{
								IpAddress = ipAddress,
								LastUpdate = DateTime.Now
							};
					}
					return true;
				//case "NTLM":
				//    var identity = iIdentity as WindowsIdentity;
				//    var ident1 = iIdentity as System.Security.Principal.WindowsPrincipal;
				//    var ident2 = iIdentity as System.Security.Principal.GenericPrincipal;
				//    var ident3 = iIdentity as System.Security.Principal.GenericIdentity;
				//    break;
				default:
					context.Disconnect("Unauthorised access.");
					WebKit.Log("Connection is unsupported from {0}@{1}", identity.Name, ipAddress ?? "HTTP");
					return false;
			}
		}

		public static void ProcessData(WebKit webKit, HttpListener listener, IAsyncResult result)
		{
			try
			{
				var context = listener.EndGetContext(result);
				var response = context.Response;
				var request = context.Request.Url.AbsolutePath;
				response.Headers.Set(HttpResponseHeader.Server, AGENT);

				var ipAddress = context.Request.RemoteEndPoint.Address.ToString();
				if (ipAddress != null)
					ipAddress = ipAddress.Split(':')[0];

				if ((request.StartsWith("/")))
					request = request.Substring(1);
				if ((request.EndsWith("/") || request.Equals(String.Empty)))
					request = request + WebServer.IndexPage;

				request = WebServer.RootPath + Path.DirectorySeparatorChar + request.Replace('/', Path.DirectorySeparatorChar);

				if (!CheckAuthenticity(context, webKit, request, ipAddress))
					return;

				if (!Json.ProcessJsonHeader(webKit, context, context.User.Identity.Name, ipAddress))
					ProcessResponse(request, context);
			}
			catch (ObjectDisposedException) { }
			catch (HttpListenerException) { }
			catch (Exception ex)
			{
				ProgramLog.Log(ex);
			}
		}

		public static void SendData(this HttpListenerContext context, string httpData, byte[] respByte)
		{
			context.Response.ContentLength64 = respByte.Length;
			context.Response.ContentType = GetContentType(httpData);
			context.Response.OutputStream.Write(respByte, 0, respByte.Length);
			context.Response.OutputStream.Close();
		}

		public static void WriteString(this HttpListenerContext ctx, string httpData, string text)
		{
			ctx.SendData(httpData, ASCIIEncoding.ASCII.GetBytes(text));
		}

		public static void WriteString(this HttpListenerContext ctx, string text)
		{
			ctx.SendData(String.Empty, ASCIIEncoding.ASCII.GetBytes(text));
		}

		public static void ProcessResponse(string requestData, HttpListenerContext context)
		{
			try
			{
				if (!File.Exists(requestData))
					context.SendData(requestData, ASCIIEncoding.ASCII.GetBytes("Sorry that page is not found."));
				else
					context.SendData(requestData, File.ReadAllBytes(requestData));
			}
			catch (Exception ex)
			{
				ProgramLog.Log(ex);
			}
		}

		public static string GetContentType(string httpData)
		{
			string extension = "";
			try
			{
				FileInfo info = new FileInfo(httpData);
				extension = info.Extension;
			}
			catch
			{
			}

			string key = extension.ToLower();
			if (key != null)
			{
				switch (key)
				{
					case "":
					case ".htm":
					case ".html":
						return "text/html; charset=UTF-8";
					case ".xhtml":
						return "text/xhtml; charset=UTF-8";
					case ".css":
						return "text/css; charset=UTF-8";
					case ".js":
						return "application/x-javascript; charset=UTF-8";
					case ".png":
						return "image/png";
					case ".gif":
						return "image/gif";
					case ".jpg":
					case ".jpeg":
						return "image/jpeg";
					case "json":
						return "application/json";
					case ".mcb":
						return "application/zip";
					case ".xml":
						return "application/xml; chatset=UTF-8";
					case ".zip":
						return "application/zip";
				}
			}
			return "application/octet-stream";
		}

		public static void Disconnect(this HttpListenerContext ctx, string message)
		{
			ctx.SendData(String.Empty, ASCIIEncoding.ASCII.GetBytes(message));
			ctx.Response.Close();
		}
	}
}