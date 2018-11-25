using System;
using Newtonsoft.Json;
using System.Text;
namespace BiscaNet.Desktop.Networking.Protocol
{
	public class Message<T>
	{
		public T Payload;

		public string Name;

		public Message(string name, T payload)
		{
			this.Name = name;
			this.Payload = payload;
		}

		public byte[] ToBytes()
		{
			var old = Encoding.UTF8.GetBytes(this.ToString());

			var res = new byte[Values.MessageLength];
			for (int i = 0; i < old.Length; i++)
			{
				res[i] = old[i];
			}

			return res;
		}

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}

		public static Message<T> Parse(string json)
		{
			return JsonConvert.DeserializeObject<Message<T>>(json);
		}
	}

	public static class Values
	{
		// Length in bytes of the message
		public const int MessageLength = 2048;
	}
}
