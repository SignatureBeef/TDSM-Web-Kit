using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebKit.Server.Utility;
using Terraria_Server.Misc;
using Terraria_Server.WorldMod;
using System.Threading;

namespace WebKit.Server.JsonData.Packets
{
	public struct Utility : IPacket
	{
		public Dictionary<String, Object> Data { get; set; }

		public PacketId GetPacket()
		{
			return PacketId.util;
		}

		public void Process(Args args)
		{
			ThreadPool.QueueUserWorkItem(Processor, args);
		}

		public void Processor(object state)
		{
			var args = (Args)state;
			var WebKit = args.WebKit;
			var ipAddress = args.IpAddress;

			lock (WebKit.ServerStatus)
			{

				/*
				 * Restart = 0
				 * Stop = 1
				 */

				int Type;
				if (Int32.TryParse(args[0].ToString(), out Type))
				{
					switch (Type)
					{
						case 0:
							Data["processed"] = Utilities.RestartServer(WebKit, ipAddress);
							Data["err"] = "Error restarting, Please check log.";
							break;
						case 1:
							Data["processed"] = Utilities.StopServer(WebKit, ipAddress);
							Data["err"] = "Error stopping, Please check log.";
							break;
						default:
							Data["processed"] = false;
							Data["err"] = "Unknown utility key.";
							break;
					}
				}
				else
				{
					Data["processed"] = false;
					Data["err"] = "Incorrect key format.";
				}
			}
		}
	}
}
