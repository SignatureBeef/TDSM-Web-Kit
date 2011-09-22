using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.JsonData
{
    public struct PacketId
    {
        public static string CHAT
        {
            get
            {
                return "chat";
            }
        }

        public static string CONFIG
        {
            get
            {
                return "config";
            }
        }
        
        public static string CREDENTIALS
        {
            get 
            {
                return "verauth";
            }
        }
        
        public static string SESSION_AUTH
        {
            get 
            {
                return "auth";
            }
        }
        
        public static string STATUS
        {
            get 
            {
                return "stats";
            }
        }
        
        public static string WEB_CHAT
        {
            get 
            {
                return "webchat";
            }
        }
        
        public static string WEB_COMMAND
        {
            get 
            {
                return "webcommand";
            }
        }

        public static string UTILITY
        {
            get
            {
                return "util";
            }
        }
    }
}
