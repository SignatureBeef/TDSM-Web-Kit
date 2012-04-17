// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using Terraria_Server.Misc;

namespace WebKit
{
    public class Properties : PropertiesFile
    {
        public Properties(string propertiesPath) : base(propertiesPath) { }

        public void PustData()
        {
            object temp = null;
            temp = MaxChatLines;
            temp = UpdateInterval;
            temp = Port;
            temp = IPAddress;
			temp = ServerId;
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

        public int UpdateInterval
        {
            get
            {
                return getValue("update-interval", 2000);
            }
            set
            {
				setValue("update-interval", value);
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

		public string ServerId
		{
			get
			{
				return getValue("server-id", String.Empty);
			}
			set
			{
				setValue("server-id", value);
			}
		}
    }
}
