// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Terraria_Server.Logging;
using System.IO;
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

        public JavaScriptSerializer Serializer = new JavaScriptSerializer();

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

		public WebServer(string ip, int port, WebKit webKit)
		{
			this.IpAddress = ip;
			this.Port = port;

			this.Setup();

			ConnectList = new Dictionary<String, String>();
			
            this.WebKit = webKit;
			this.SessionTime = webKit.Properties.UpdateInterval;
		}

		private void Setup()
		{
			this.HttpListener = new HttpListener();
			this.HttpListener.Prefixes.Add(String.Format("http://{0}:{1}/", this.IpAddress, this.Port));
			this.HttpListener.IgnoreWriteExceptions = true;
			this.HttpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
		}

		public void StartServer()
		{
			if (!this.HttpListener.IsListening)
				this.Setup();

			this.HttpListener.Start();			
			ThreadPool.QueueUserWorkItem(new WaitCallback(Listen));

			WebKit.Log("WebKit Server started on {0}", this.HttpListener.Prefixes.ToArray()[0]);
		}

        public void Listen(object state)
        {
            try
            {
				while (this.HttpListener.IsListening)
				{
					IAsyncResult result = this.HttpListener.BeginGetContext(new AsyncCallback(ListenerCallback), this.HttpListener);
					result.AsyncWaitHandle.WaitOne();
                }
            }
            catch { }
        }

        public void StopServer()
        {
            try
			{
				this.HttpListener.Close();
			}
            catch (Exception e) { ProgramLog.Log(e); }
        }

        public void ListenerCallback(IAsyncResult result)
        {
			try
			{
				var listener = result.AsyncState as HttpListener;
				Html.ProcessData(this.WebKit, listener, result);
			}
			catch (ObjectDisposedException) { }
			catch (Exception e)
			{
				ProgramLog.Log(e);
			}
        }
    }
}
