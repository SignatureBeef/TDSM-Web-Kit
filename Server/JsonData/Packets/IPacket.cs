using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.Script.Serialization;
using WebKit.Server.Utility;

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

		void Process(Args args);
	}

	public struct Args
	{
		//public WebSender Sender { get; set; }
		public WebKit WebKit { get; set; }
		public string AuthName { get; set; }
		public string IpAddress { get; set; }
		public object[] Arguments { get; set; }

		public int Count
		{
			get { return Arguments.Length; }
		}

		public object this[int index]
		{
			get { return Arguments[index]; }
			set { Arguments[index] = value; }
		}

		public object ElementAt(int index)
		{
			return Arguments[index];
		}
	}

	//public interface SerializablePacket : IPacket
	//{
	//    string ToJSON();
	//}

	public static class IPacketExtensions
	{
		public static JavaScriptSerializer Serializer { get; set; }

		static IPacketExtensions()
		{
			Serializer = new JavaScriptSerializer();
		}

		public static string ToJSON(this IPacket packet)
	    {
	        //var obj = packet.Data.Take(packet.Data.Count).ToArray();
	        //var obj = packet.Data.FieldwiseClone();
			lock(packet)
				return Serializer.Serialize(packet.Data);
	    }
	}
}
