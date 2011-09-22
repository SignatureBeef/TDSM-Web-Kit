using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData
{
    public struct WebMessage
    {
        public string sender;
        public string message;
        public string rank;
        public DateTime timesent;

        public WebMessage(string Sender, string Message, string Rank, DateTime Time)
        {
            sender = Sender;
            message = Message;
            rank = Rank;
            timesent = Time;
        }
    }
}
