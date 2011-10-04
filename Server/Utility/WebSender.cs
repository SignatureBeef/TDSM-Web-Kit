using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;

namespace WebKit.Server.Utility
{
    public class WebSender : Sender
    {
        WebKit _WebKit { get; set; }

        public WebSender(WebKit WebKit)
        {
            _WebKit = WebKit;

            Op = true;
            Name = "WEB";
        }

        public override void sendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            _WebKit.AddChatLine(Message);
        }
    }
}
