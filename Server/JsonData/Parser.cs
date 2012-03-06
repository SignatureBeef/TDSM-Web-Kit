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
				message.Data = new Dictionary<String, Object>();

				array.Add(message);
			}

			return array;
		}

		public static string ProcessPacket(string Id, Args args)
		{
			try
			{
				foreach (IPacket packet in Packets.Where(x => x.GetPacket().ToString().Equals(Id)))
				{
					packet.Process(args);
					return packet.ToJSON();
				}
			}
			catch (ExitException) { throw; }
			catch (Exception e)
			{
				ProgramLog.Log(e);
			}

			return null;
		}

		public static void RemoveFirst(ref string[] args)
		{
			//lock (args)
			{
				var count = args.Count();
				if (count > 0)
				{
					args = args.Skip(1).ToArray();
				}
			}
		}

		public static void InsertAtFirst(ref string[] args, string arg)
		{
			var newArgs = (string[])args.Clone();
			var len = newArgs.Length;

			Array.Resize(ref newArgs, len);
			newArgs = (string[])newArgs.Reverse();

			newArgs[len] = arg;
			args = (string[])newArgs.Reverse();
		}

		public static void ParseAndProcess(WebKit webKit, HttpListenerContext context, string[] args, string user, string ipAddress)
		{
			try
			{
				if (args != null && args.Length > 0)
				{
					//paremeters.Clear();

					string Id = args.ElementAt(0).Trim();

					if (Id != null && Id.Length > 0)
					{
						RemoveFirst(ref args);

						for (var i = 0; i < args.Length; i++)
						{
							var arg = args[i];
							if (arg is String)
							{
								string text = arg.Trim();
								if (text.Length > 0)
								{
									if (text.Contains('='))
										text = text.Remove(0, text.IndexOf('=') + 1);

									args[i] = text;
								}
							}
						}

						var arguments = new Args()
						{
							Arguments = args,
							AuthName = user,
							IpAddress = ipAddress,
							WebKit = webKit
						};

						//InsertAtFirst(
						//Data.Insert(0, IPAddress);

						//foreach (string param in Data)
						//{
						//    string parameter = param.Trim();
						//    if (parameter.Length > 0)
						//    {
						//        if (parameter.Contains('='))
						//        {
						//            parameter = parameter.Remove(0, parameter.IndexOf('=') + 1);
						//        }
						//        paremeters.Add(parameter);
						//    }
						//}

						//paremeters.Insert(0, WebKit);

						//var args = new Args()
						//{
						//    WebKit = WebKit,
						//    Sender = new Utility.WebSender(
						//};




						var serialized = ProcessPacket(Id, arguments);
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
