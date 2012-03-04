using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebKit.Server.JsonData.Packets;
using Terraria_Server.Logging;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Terraria_Server.Misc;

namespace WebKit.Server.JsonData
{
    public static class Parser
    {
        public static List<IPacket> Packets = GetPackets();

        public static List<IPacket> GetPackets()
        {
            List<IPacket> array = new List<IPacket>();

            Type type = typeof(IPacket);
            foreach (Type messageType in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(clazz => clazz.GetTypes()).Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
            {
                IPacket message = (IPacket)Activator.CreateInstance(messageType);

                array.Add(message);
            }

            return array;
        }

        public static Dictionary<String, Object> ProcessPacket(string Id, object[] Data)
        {
			try
			{
				foreach (IPacket packet in Packets.Where(x => x.GetPacket().Equals(Id)))
					return packet.Process(Data);
			}
			catch (ExitException) { throw; }
			catch (Exception e)
			{
				ProgramLog.Log(e);
			}

            return null;
        }

        public static void ParseAndProcess(WebKit WebKit, HttpListenerContext context, List<String> Data)
        {
			try
			{
				List<Object> paremeters = WebKit.WebServer.parameters;
				if (Data != null && Data.Count > 0)
				{
					paremeters.Clear();

					string Id = Data.ToArray()[0].Trim();
					string IPAddress = context.Request.UserHostAddress.Split(':')[0];

					if (Id != null && Id.Length > 0)
					{
						Data.RemoveAt(0);

						Data.Insert(0, IPAddress);

						foreach (string param in Data)
						{
							string parameter = param.Trim();
							if (parameter.Length > 0)
							{
								if (parameter.Contains('='))
								{
									parameter = parameter.Remove(0, parameter.IndexOf('=') + 1);
								}
								paremeters.Add(parameter);
							}
						}

						paremeters.Insert(0, WebKit);

						var array = ProcessPacket(Id, paremeters.ToArray<Object>());

						if (array == null)
							return;

						string serialized = Json.Serialize(WebKit.WebServer.serializer, array);
						//Html.SendData(String.Empty, context, Encoding.ASCII.GetBytes(serialized));
						context.WriteString(String.Empty, serialized);
					}
				}
			}
			catch (ExitException) { throw; }
			catch (HttpListenerException) { }
			catch (ArgumentException) { }
			catch (Exception e)
			{
				ProgramLog.Log(e);
			}            
        }
    }
}
