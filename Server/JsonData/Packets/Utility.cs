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
		public string GetPacket()
		{
			return PacketId.UTILITY;
		}

		public Dictionary<String, Object> Process(object[] Data)
		{
			Dictionary<String, Object> array = new Dictionary<String, Object>();

			WebKit WebKit = (WebKit)Data[0];
			string IPAddress = Data[1].ToString();

			/*
			 * Restart = 0
			 * Stop = 1
			 */

			int Type;
			if (Int32.TryParse(Data[2].ToString(), out Type))
			{
				switch (Type)
				{
					case 0:
						array["processed"] = Utilities.RestartServer(WebKit, IPAddress);
						array["err"] = "Error restarting, Please check log.";
						break;
					case 1:
						array["processed"] = Utilities.StopServer(WebKit, IPAddress);
						array["err"] = "Error stopping, Please check log.";
						break;
					default:
						array["processed"] = false;
						array["err"] = "Unknown utility key.";
						break;
				}
			}
			else
			{
				array["processed"] = false;
				array["err"] = "Incorrect key format.";
			}

			return array;
		}
	}
}
