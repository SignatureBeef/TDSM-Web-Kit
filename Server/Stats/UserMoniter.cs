// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Collections.Generic;
using Terraria_Server;
using System.Reflection;
using Terraria_Server.Misc;

namespace WebKit.Server.Stats
{
    public static class UserMoniter
    {
        public static List<String> SerializePlayer(Player player)
        {
            List<String> data = new List<String>();

            foreach (PropertyInfo info in player.GetType().GetProperties())
            {
                try
                {
                    string pInfo = info.Name + ":";
					//Type mType = info.GetValue(player, null).GetType();

                    object variable = info.GetValue(player, null);
                    if(variable is Vector2)
                    {
                        Vector2 vVar = (Vector2)variable;
                        data.Add(pInfo + variable.ToString() + " {" + vVar.X.ToString() + ", " + vVar.Y.ToString() + "}");
                    }
                    else
                        data.Add(pInfo + variable.ToString());
                }
                catch { }
            }

            return data;
        }
    }
}
