using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace WebKit
{
    public class Properties : PropertiesFile
    {
        public Properties(string propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = null;
            temp = MaxChatLines;
            temp = SessionMinutes;
            temp = Port;
            temp = IPAddress;
        }

        public int MaxChatLines
        {
            get
            {
                return getValue("max-chat-lines", 300);
            }
            set
            {
                setValue("max-chat-lines", value);
            }
        }

        public int SessionMinutes
        {
            get
            {
                return getValue("max-session-time", 15);
            }
            set
            {
                setValue("max-session-time", value);
            }
        }

        public int Port
        {
            get
            {
                return getValue("port", 7775);
            }
            set
            {
                setValue("port", value);
            }
        }

        public string IPAddress
        {
            get
            {
                return getValue("ip-address", "*");
            }
            set
            {
                setValue("ip-address", value);
            }
        }
    }
}
