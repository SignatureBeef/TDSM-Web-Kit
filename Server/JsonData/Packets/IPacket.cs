using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.Script.Serialization;

namespace WebKit.Server.JsonData.Packets
{
	//public interface IPacket
	//{
	//    string GetPacket();

	//    Dictionary<String, Object> Process(object[] Data);
	//}

	public interface IPacket
	{
		Dictionary<String, Object> Data { get; set; }
		PacketId GetPacket();

		void Process(object[] data);
	}

	public static class IPacketExtensions
	{
		public static JavaScriptSerializer Serializer { get; set; }

		static IPacketExtensions()
		{
			Serializer = new JavaScriptSerializer();
		}

		public static string ToJSON(this IPacket packet)
		{
			lock (packet.Data)
			{
				return Serializer.Serialize(packet.Data);
			}
		}
	}
}
