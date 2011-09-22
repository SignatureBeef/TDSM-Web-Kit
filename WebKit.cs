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

namespace WebKit
{
    public class WebKit : BasePlugin
    {
        public WebServer                         WebServer       { get; set; }
        public Dictionary<String, WebMessage>    UserChat        { get; set; }
        public Dictionary<String, DateTime>      WebSessions     { get; set; }
        public List<Credential>                  CredentialList  { get; set; }
        public Properties                        Properties      { get; set; }
        public string                            ServerStatus    { get; set; }

        public static string PluginPath
        {
            get
            {
                return Statics.PluginPath + Path.DirectorySeparatorChar + "WebKit";
            }
        }

        protected override void Initialized(object state)
        {
            base.Name = "WebKit";
            base.Description = "A Web Management plugin for TDSM";
            base.Author = "DeathCradle";
            base.Version = "1";
            base.TDSMBuild = 36;

            ServerStatus = "Online";

            if (!Directory.Exists(PluginPath))
                Directory.CreateDirectory(PluginPath);

            if (!Directory.Exists(WebServer.RootPath))
                Directory.CreateDirectory(WebServer.RootPath);

            Properties = new Properties(PluginPath + Path.DirectorySeparatorChar + "WebKit.config");
            Properties.Load();
            Properties.pushData();
            Properties.Save(false);

            Authentication.Init();
        }

        protected override void Enabled()
        {
            UserChat = new Dictionary<String, WebMessage>();
            CredentialList = Authentication.GetCredentials();
            WebSessions = new Dictionary<String, DateTime>();

            Hook(HookPoints.PlayerChat, OnPlayerChat);
            Hook(HookPoints.PlayerEnteredGame, OnPlayerJoin);
            Hook(HookPoints.PlayerLeftGame, OnPlayerDisconnect);

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

        void OnPlayerChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
        {
            UserChat.Add(DateTime.Now.ToBinary().ToString(),
                new WebMessage(ctx.Sender.Name, args.Message, (ctx.Sender.Op) ? "OP" : "RU", DateTime.Now));
        }

        void OnPlayerJoin(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        {
            UserChat.Add(DateTime.Now.ToBinary().ToString(),
                new WebMessage("Server", String.Format(ctx.Sender.Name + " connected from {0}", ctx.Player.IPAddress), "", DateTime.Now));
        }

        void OnPlayerDisconnect(ref HookContext ctx, ref HookArgs.PlayerLeftGame args)
        {
            UserChat.Add(DateTime.Now.ToBinary().ToString(),
                new WebMessage("Server", String.Format(ctx.Sender.Name + " diconnected.", ctx.Player.IPAddress), "", DateTime.Now));
        }
    }
}
