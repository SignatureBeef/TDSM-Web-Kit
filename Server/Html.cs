using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using Terraria_Server.Logging;
using WebKit.Server.JsonData;
using Terraria_Server;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using WebKit.Server.Auth;

namespace WebKit.Server
{
	public struct TDSMBasicIdentity //: IIdentity
	{
		//public TDSMBasicIdentity(string username, string password) : base(username, password) { }

		public WebKit WebKit { get; set; }

		public string Name { get; set; }
		public string Password { get; set; }

		public AuthStatus AuthStatus
		{
			get
			{
				return Authentication.IsCredentialsTheSame(Name, Password, WebKit);
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return AuthStatus == AuthStatus.MATCH;
			}
		}
	}

    public static class Html
    {
		public const String AGENT = "TDSM WebKit";

		public static TDSMBasicIdentity ToTDSMIdentity(this HttpListenerBasicIdentity identity, WebKit WebKit)
		{
			//var ident = identity as TDSMBasicIdentity;
			//ident.WebKit = WebKit;

			return new TDSMBasicIdentity()
			{
				WebKit = WebKit,
				Name = identity.Name,
				Password = identity.Password
			};
		}

        public static void ProcessData(WebKit WebKit, HttpListener Listener, IAsyncResult Result)
        {
            try
            {
                var context = Listener.EndGetContext(Result);
				var response = context.Response;
				var request = context.Request.Url.AbsolutePath;

				response.Headers.Set(HttpResponseHeader.Server, AGENT);

				var identity = context.User.Identity;
				switch (identity.AuthenticationType)
				{
					case "Basic":
						var basicIdentity = (identity as HttpListenerBasicIdentity).ToTDSMIdentity(WebKit);

						if (basicIdentity.AuthStatus != AuthStatus.MATCH)
						{
							var ipAddress = context.Request.UserHostAddress;

							if(ipAddress != null)
								ipAddress = ipAddress.Split(':')[0];

							WebKit.Log("{0} disconnected from {1}", basicIdentity.Name, ipAddress ?? "HTTP");
							context.Disconnect("Credentials incorrect.");
							return;
						}

						break;
					//case "NTLM":
					//    var identity = iIdentity as WindowsIdentity;
					//    var ident1 = iIdentity as System.Security.Principal.WindowsPrincipal;
					//    var ident2 = iIdentity as System.Security.Principal.GenericPrincipal;
					//    var ident3 = iIdentity as System.Security.Principal.GenericIdentity;
					//    break;
					default:
						context.Disconnect("Unauthorised access.");
					    return;
				}

				//var identity = context.User.Identity as HttpListenerBasicIdentity;

				//var pass = identity.Password;

				//if (!identity.IsAuthenticated)
				//{
				//    SendData(String.Empty, context, ASCIIEncoding.ASCII.GetBytes("Not authorised!"));
				//    context.Response.Close();
				//    return;
				//}


                if ((request.StartsWith("/")))
                {
                    request = request.Substring(1);
                }

                if ((request.EndsWith("/") || request.Equals("")))
                {
                    request = request + WebServer.IndexPage;
                }

                request = WebServer.RootPath + Path.DirectorySeparatorChar + request.Replace('/', Path.DirectorySeparatorChar);

                if (!Json.ProcessJsonHeader(WebKit, context))
                    ProcessResponse(request, context);

            }
            catch (ObjectDisposedException) { }
            catch (HttpListenerException) { }
            catch (Exception ex)
            {
                ProgramLog.Log(ex);
            }
        }

        public static void SendData(string httpData, HttpListenerContext context, byte[] respByte)
        {
            context.Response.ContentLength64 = respByte.Length;
            context.Response.ContentType = GetContentType(httpData);
            context.Response.OutputStream.Write(respByte, 0, respByte.Length);
            context.Response.OutputStream.Close();
        }

        public static void ProcessResponse(string requestData, HttpListenerContext context)
        {
            try
            {
                if (!File.Exists(requestData))
                    SendData(requestData, context, ASCIIEncoding.ASCII.GetBytes("Sorry that page is not found."));
                else
                    SendData(requestData, context, File.ReadAllBytes(requestData));
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
            catch { }

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

		public static void Disconnect(this HttpListenerContext ctx, string Message)
		{
			SendData(String.Empty, ctx, ASCIIEncoding.ASCII.GetBytes(Message));
			ctx.Response.Close();
		}
    }
}
