// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Terraria_Server.Logging;

namespace WebKit.Server.JsonData.Packets
{
	public interface IPacket
	{
		PacketId GetPacket();

		void Process(Args args);
	}

	public struct Args
	{
		public WebKit WebKit { get; set; }
		public string AuthName { get; set; }
		public string IpAddress { get; set; }
		public object[] Arguments { get; set; }

		public int Count
		{
			get { return this.Arguments.Length; }
		}

		public object this[int index]
		{
			get { return this.Arguments[index]; }
			set { this.Arguments[index] = value; }
		}

		public object ElementAt(int index)
		{
			return this.Arguments[index];
		}
	}

	public abstract class SerializablePacket : IPacket
	{
		public static JavaScriptSerializer Serializer { get; set; }

		static SerializablePacket()
		{
			Serializer = new JavaScriptSerializer();
		}

		private Dictionary<String, Object> _data;
		public Dictionary<String, Object> Data
		{
			get
			{
				if (_data == null)
					_data = Init();

				lock (_data)
					return _data;
			}
			set
			{
				if (_data == null)
					_data = Init();

				lock (_data)
					_data = value;
			}
		}

		public Dictionary<String, Object> Init()
		{
			return new Dictionary<String, Object>();
		}

		public string ToJson()
		{
			try
			{
				if (_data == null)
					_data = Init();

				lock (_data) { return Serializer.Serialize(_data); }
			}
			catch (Exception e)
			{
				ProgramLog.Log("Exception in {0}", GetPacket().ToString());
				ProgramLog.Log(e);
				return String.Empty;
			}
		}

		public abstract PacketId GetPacket();

		public abstract void Process(Args args);
	}
}