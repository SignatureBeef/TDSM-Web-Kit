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

namespace WebKit.Server
{
    public class Html
    {
        public static void ProcessData(WebKit WebKit, HttpListener Listener, IAsyncResult Result)
        {
            try
            {
                HttpListenerContext context = Listener.EndGetContext(Result);
                HttpListenerResponse response = context.Response;
                string request = context.Request.Url.AbsolutePath;

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
    }
}
