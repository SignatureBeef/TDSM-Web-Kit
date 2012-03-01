using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Logging;
using WebKit.Server;
using System.IO;
using Terraria_Server;
using WebKit.Server.JsonData;
using WebKit.Server.Auth;
using System.Web.Script.Serialization;
using Terraria_Server.Plugins;
using System.Diagnostics;
using WebKit.Server.Utility;
using WebKit.Server.Misc;

namespace WebKit
{
	public class WebKit : BasePlugin
	{
		public WebServer WebServer { get; set; }
		public MultiArray<String, WebMessage> UserChat { get; set; }
		public Dictionary<String, DateTime> WebSessions { get; set; }
		public List<Credential> CredentialList { get; set; }
		public Properties Properties { get; set; }
		public string ServerStatus { get; set; }
		public WebSender WebSender { get; set; }

		public static string PluginPath
		{
			get
			{
				return Statics.PluginPath + Path.DirectorySeparatorChar + "WebKit";
			}
		}

		public WebKit()
		{
			Name = "WebKit";
			Description = "A Web Management plugin for TDSM";
			Author = "DeathCradle";
			Version = "1";
			TDSMBuild = 37;
		}

		protected override void Initialized(object state)
		{
			ServerStatus = "Online";

			if (!Directory.Exists(PluginPath))
				Directory.CreateDirectory(PluginPath);

			if (!Directory.Exists(WebServer.RootPath))
				Directory.CreateDirectory(WebServer.RootPath);

			Properties = new Properties(PluginPath + Path.DirectorySeparatorChar + "WebKit.config");
			Properties.Load();
			Properties.pushData();

			if (Properties.ServerId == String.Empty)
				Properties.ServerId = "tdsm-" + Main.rand.Next(0, Int32.MaxValue);

			Properties.Save(false);

			Authentication.Init();
		}

		protected override void Enabled()
		{
			UserChat = new MultiArray<String, WebMessage>();
			CredentialList = Authentication.GetCredentials();
			WebSessions = new Dictionary<String, DateTime>();

			WebSender = new WebSender(this);

			//Hook(HookPoints.PlayerChat, OnPlayerChat);
			//Hook(HookPoints.PlayerEnteredGame, OnPlayerJoin);
			//Hook(HookPoints.PlayerLeftGame, OnPlayerDisconnect);
			//Hook(HookPoints.ConsoleMessageReceived, OnConsoleMessageReceived);

			WebServer = new WebServer(Properties.IPAddress, Properties.Port, this);
			WebServer.StartServer();

			ProgramLog.Plugin.Log("WebKit for TDSM #{0} enabled.", base.TDSMBuild);
		}

		protected override void Disabled()
		{
			WebServer.StopServer();

			ProgramLog.Plugin.Log("WebKit disabled.");
		}

		public static void Log(string fmt, params object[] args)
		{
			foreach (string line in String.Format(fmt, args).Split('\n'))
			{
				ProgramLog.Plugin.Log("[WebKit] " + line);
			}
		}

		[Hook(HookOrder.NORMAL)]
		void OnPlayerChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
		{
			AddChatLine(args.Message, ctx.Sender.Name, ctx.Sender.Op ? "OP" : "RU");
		}

		[Hook(HookOrder.NORMAL)]
		void OnPlayerJoin(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
		{
			AddChatLine(String.Format(ctx.Sender.Name + " connected from {0}", ctx.Player.IPAddress));
		}

		[Hook(HookOrder.NORMAL)]
		void OnPlayerDisconnect(ref HookContext ctx, ref HookArgs.PlayerLeftGame args)
		{
			AddChatLine(ctx.Sender.Name + " diconnected.");
		}

		[Hook(HookOrder.NORMAL)]
		void OnConsoleMessageReceived(ref HookContext ctx, ref HookArgs.ConsoleMessageReceived args)
		{
			var prefix = String.Empty;
			if (!(ctx.Sender is ConsoleSender))
				prefix = ctx.Sender.Op ? "OP" : "RU";

			if (args.Logger == ProgramLog.SendingLogger.PLAYER)
				return;

			AddChatLine(args.Message, ctx.Sender.Name, prefix);
		}

		public void AddChatLine(string ServerMessage, string Sender = "Server", string Rank = "")
		{
			string time = DateTime.Now.ToBinary().ToString();
			if (UserChat.ContainsKey(time))
				time = DateTime.Now.AddMilliseconds(-1).ToBinary().ToString();

			UserChat.Add(time,
				new WebMessage(Sender, ServerMessage.Trim(), Rank, DateTime.Now));
		}
	}
}
