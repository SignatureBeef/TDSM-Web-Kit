// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;

namespace WebKit.Server.JsonData
{
    public struct WebMessage
    {
        public string Sender;
        public string Message;
        public string Rank;
        public DateTime TimeSent;

        public WebMessage(string sender, string message, string rank, DateTime timeSent)
        {
            Sender = sender;
            Message = message;
            Rank = rank;
            TimeSent = timeSent;
        }
    }
}
