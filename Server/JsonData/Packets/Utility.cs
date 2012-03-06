// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using WebKit.Server.Utility;
using System.Threading;

namespace WebKit.Server.JsonData.Packets
{
	public class Utility : SerializablePacket
	{
		public override PacketId GetPacket()
		{
			return PacketId.Util;
		}

		public override void Process(Args args)
		{
			ThreadPool.QueueUserWorkItem(Processor, args);
		}

		public void Processor(object state)
		{
			var args = (Args)state;
			var webKit = args.WebKit;
			var ipAddress = args.IpAddress;

			lock (webKit.ServerStatus)
			{

				/*
				 * Restart = 0
				 * Stop = 1
				 */

				int type;
				if (Int32.TryParse(args[0].ToString(), out type))
				{
					switch (type)
					{
						case 0:
							Data["processed"] = Utilities.RestartServer(webKit, ipAddress);
							Data["err"] = "Error restarting, Please check log.";
							break;
						case 1:
							Data["processed"] = Utilities.StopServer(webKit, ipAddress);
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
