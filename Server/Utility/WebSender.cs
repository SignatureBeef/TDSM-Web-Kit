// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using Terraria_Server;

namespace WebKit.Server.Utility
{
    public class WebSender : Sender
    {
		public WebKit WebKit { get; set; }

        public WebSender(WebKit webKit)
        {
            WebKit = webKit;

            Op = true;
            Name = "WEB";
        }

        public override void sendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            WebKit.AddChatLine(Message);
        }
    }
}
