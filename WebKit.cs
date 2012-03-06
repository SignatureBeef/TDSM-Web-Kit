// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Collections.Generic;
using Terraria_Server.Logging;
using WebKit.Server;
using System.IO;
using Terraria_Server;
using WebKit.Server.JsonData;
using WebKit.Server.Auth;
using Terraria_Server.Plugins;
using WebKit.Server.Utility;
using WebKit.Server.Misc;
using Terraria_Server.Commands;
using WebKit.Server.JsonData.Packets;

namespace WebKit
{
	public partial class WebKit : BaseWebKit
	{
		public WebServer WebServer { get; set; }
		public MultiArray<String, WebMessage> UserChat { get; set; }
		public Dictionary<String, Identity> WebSessions { get; set; }
		public List<Credential> CredentialList { get; set; }
		public Properties Properties { get; set; }
		public string ServerStatus { get; set; }
		public WebSender WebSender { get; set; }
		public int MainUpdateInterval { get; set; }
		public List<KeyValuePair<String, Identity>> KickList { get; set; }

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
			Properties.PustData();

			if (Properties.ServerId == String.Empty)
				Properties.ServerId = "tdsm-" + Main.rand.Next(0, Int32.MaxValue);

			Properties.Save(false);

			MainUpdateInterval = Properties.UpdateInterval;

			Authentication.Init(Properties.ServerId);
		}

		protected override void Enabled()
		{
			UserChat = new MultiArray<String, WebMessage>();
			CredentialList = Authentication.GetCredentials();
			WebSessions = new Dictionary<String, Identity>();
			KickList = new List<KeyValuePair<String, Identity>>();

			WebSender = new WebSender(this);

			//Hook(HookPoints.PlayerChat, OnPlayerChat);
			//Hook(HookPoints.PlayerEnteredGame, OnPlayerJoin);
			//Hook(HookPoints.PlayerLeftGame, OnPlayerDisconnect);
			//Hook(HookPoints.ConsoleMessageReceived, OnConsoleMessageReceived);

			AddCommand("webserver")
				.WithAccessLevel(AccessLevel.CONSOLE)
				.WithDescription("Manage the WebKit Http Server.")
				.WithHelpText("Usage:    webserver stop:start:restart")
				.Calls(Server);
			AddCommand("webconnection")
				.WithAccessLevel(AccessLevel.CONSOLE)
				.WithDescription("Manage Http connections to WebKit.")
				.WithHelpText("Usage:    connection view")
				.WithHelpText("          connection kick <id>")
				.Calls(Connection);
			AddCommand("webusers")
				.WithAccessLevel(AccessLevel.CONSOLE)
				.WithDescription("Manage Htp users.")
				.WithHelpText("Usage:    webusers add <user> <pass>")
				.WithHelpText("          webusers remove <user>")
				.WithHelpText("          webusers reloadconfig")
				.Calls(Users);

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
				ProgramLog.Plugin.Log("[WebKit] " + line);
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

			if (args.Logger == SendingLogger.PLAYER || args.Logger == (SendingLogger)SendingLoggerExtension.WEB)
				return;

			AddChatLine(args.Message, ctx.Sender.Name, prefix);
		}

		public void AddChatLine(string serverMessage, string sender = "Server", string rank = "")
		{
			string time = DateTime.Now.ToBinary().ToString();
			if (UserChat.ContainsKey(time))
				time = DateTime.Now.AddMilliseconds(-1).ToBinary().ToString();

			UserChat.Add(time,
				new WebMessage(sender, serverMessage.Trim(), rank, DateTime.Now));
		}
	}
}
