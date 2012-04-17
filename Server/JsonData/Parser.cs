// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Linq;
using WebKit.Server.JsonData.Packets;
using Terraria_Server.Logging;
using Terraria_Server.Misc;
using System.Net;

namespace WebKit.Server.JsonData
{
	public static class Parser
	{
		public static SerializablePacket[] Packets { get; set; }

		static Parser()
		{
			Packets = GetPackets();
		}

		public static SerializablePacket[] GetPackets()
		{
			SerializablePacket[] array = new SerializablePacket[GetMaxPackets() + 1];

			Type type = typeof(SerializablePacket);
			foreach (Type messageType in AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(clazz => clazz.GetTypes()).Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
			{
				SerializablePacket message = (SerializablePacket)Activator.CreateInstance(messageType);
				//message.Data = new Dictionary<String, Object>();

				array[(int)message.GetPacket()] = message;
			}

			return array;
		}

		private static int GetMaxPackets()
		{
			var enumValues = Enum.GetValues(typeof(PacketId));
			var max = 0;

			foreach (var val in enumValues)
			{
				var enumValue = (int)val;
				if (enumValue > max)
					max = enumValue;
			}

			return max;
		}

		public static string ProcessPacket(string id, Args args)
		{
			try
			{
				foreach (SerializablePacket packet in Packets.Where(x => x.GetPacket().ToString().ToLower().Equals(id.ToLower())))
				{
					packet.Process(args);
					return packet.ToJson();
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

					string id = args.ElementAt(0).Trim();

					if (id != null && id.Length > 0)
					{
						RemoveFirst(ref args);

						for (var i = 0; i < args.Length; i++)
						{
							var arg = args[i].ToString();
							if (arg.Length > 0)
							{
								if (arg.Contains('='))
									arg = arg.Remove(0, arg.IndexOf('=') + 1).Trim();

								if (arg.Length > 0)
									args[i] = arg;
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




						var serialized = ProcessPacket(id, arguments);
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
