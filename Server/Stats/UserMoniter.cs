using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using Terraria_Server.Misc;

namespace WebKit.Server.Stats
{
    public class UserMoniter
    {
        public static List<String> SerializePlayer(Player Player)
        {
            List<String> data = new List<String>();

            foreach (PropertyInfo info in Player.GetType().GetProperties())
            {
                try
                {
                    string pInfo = info.Name + ":";
                    Type mType = info.GetValue(Player, null).GetType();
                    //if (mType.IsAnsiClass)
                    //{
                    //    try
                    //    {
                    //        foreach (PropertyInfo propertyInfo in mType.GetProperties())
                    //        {
                    //            data.Add("   " + pInfo + propertyInfo.GetValue(info.GetValue(Player, null), null).ToString());
                    //        }
                    //    }
                    //    catch { }
                    //}
                    //else
                    object variable = info.GetValue(Player, null);
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
