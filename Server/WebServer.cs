using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Terraria_Server.Logging;
using System.IO;
using Terraria_Server;
using WebKit.Server.JsonData;
using WebKit.Server.Auth;
using System.Web.Script.Serialization;

namespace WebKit.Server
{
    public class WebServer
    {
        HttpListener        HttpListener        { get; set; }
        public string       IpAddress           { get; set; }
        public int          Port                { get; set; }
        
        public int          SessionTime         { get; set; }
        public WebKit       WebKit              { get; set; }

        public List<Object> parameters = new List<Object>();
        public JavaScriptSerializer serializer = new JavaScriptSerializer();

        public static Dictionary<String, String> ConnectList { get; set; }
        
        public static string RootPath
        {
            get
            {
                return WebKit.PluginPath + Path.DirectorySeparatorChar + "Web";
            }
        }

        public static string IndexPage
        {
            get
            {
                return "index.html";
            }
        }

        public WebServer(string ip, int port, WebKit WebKit)
        {
			IpAddress = ip;
			Port = port;

			Setup();

			ConnectList = new Dictionary<String, String>();
			
            this.WebKit = WebKit;
            SessionTime = WebKit.Properties.SessionMinutes;
        }

		private void Setup()
		{
			HttpListener = new HttpListener();
			HttpListener.Prefixes.Add(String.Format("http://{0}:{1}/", IpAddress, Port));
			HttpListener.IgnoreWriteExceptions = true;
			HttpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
		}

        public void StartServer()
        {
			if (!HttpListener.IsListening)
				Setup();

			HttpListener.Start();			
            ThreadPool.QueueUserWorkItem(new WaitCallback(Listen));

            WebKit.Log("WebKit Server started on {0}", HttpListener.Prefixes.ToArray()[0]);
        }


        public void Listen(object state)
        {
            try
            {
                while (HttpListener.IsListening)
                {
                    IAsyncResult result = HttpListener.BeginGetContext(new AsyncCallback(ListenerCallback), HttpListener);
                    result.AsyncWaitHandle.WaitOne();
                }
            }
            catch { }
        }

        public void StopServer()
        {
            try
            {
                HttpListener.Close();
            }
            catch (Exception e) { ProgramLog.Log(e); }
        }

        public void ListenerCallback(IAsyncResult result)
        {
            try
            {
				var listener = result.AsyncState as HttpListener;
				Html.ProcessData(WebKit, listener, result);
            }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
                ProgramLog.Log(e);
            }
        }
    }
}
