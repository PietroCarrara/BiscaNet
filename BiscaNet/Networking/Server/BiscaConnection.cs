using System;
using System.Net.Sockets;
using BiscaNet.Desktop.Networking.Protocol;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using BiscaNet.Desktop.Data;
using System.Threading;
using System.Diagnostics;

namespace BiscaNet.Desktop.Networking.Server
{
	public class BiscaConnection
	{
		private Socket conn;
		private BiscaServer parent;

		public int Index { get; private set; } = -1;
		public string Name { get; private set; } = "";

		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		public BiscaConnection(Socket conn, BiscaServer parent)
		{
			this.conn = conn;
			this.parent = parent;

			Task.Run(() =>
			{
				Console.WriteLine("BiscaConnection[" + this.Index + "]: Listening for messages!");
				try
				{
					listen();
					Debug.WriteLine("BiscaConnection[" + this.Index + "]: Stopped");
				}
				catch (Exception e)
				{
					Console.WriteLine("BiscaConnection[" + this.Index + "]: Error while wating for connections!: " + e.Message);
					Console.WriteLine("BiscaConnection[" + this.Index + "]: Closing connection...");
					// TODO: Close connection
				}
			}, cancellationTokenSource.Token);
		}

		// Has this player sent all config data?
		public bool Ready
		{
			get => this.Index != -1 && this.Name != "";
		}

		public void SetGameReady(bool b)
		{
			var msg = new Message<bool>("SetGameReady", b);

			conn.Send(msg.ToBytes());
		}

		public void SetIndex(int index)
		{
			this.Index = index;
			var msg = new Message<int>("SetIndex", index);

			conn.Send(msg.ToBytes());
		}

		public void AddPlayer(int zone, int idx)
		{
			var msg = new Message<Tuple<int, int>>("AddPlayer", new Tuple<int, int>(zone, idx));

			conn.Send(msg.ToBytes());
		}

		public void SetStartingPlayer(int player)
		{
			var msg = new Message<int>("SetStartingPlayer", player);

			conn.Send(msg.ToBytes());
		}

		private void listen()
		{
			while (parent.Running)
			{
				if (conn.Available >= Protocol.Values.MessageLength)
				{
					var bytes = new byte[Protocol.Values.MessageLength];
					conn.Receive(bytes);

					var str = Encoding.UTF8.GetString(bytes);
					var msg = Message<object>.Parse(str);

					Console.WriteLine("BiscaConnection[" + this.Index + "]: message received: " + msg);

					switch (msg.Name)
					{
						case "SetName":
							this.Name = Convert.ToString(msg.Payload);
							break;
						case "PlayCard":
							playedCard = Convert.ToInt32(msg.Payload);
							break;
						case "SendChat":
							parent.SendChat(this.GetName(), Convert.ToString(msg.Payload));
							break;
					}
				}
			}
		}

		private int playedCard = -1;
		public int PlayCard()
		{
			var res = playedCard;
			playedCard = -1;
			return res;
		}

		public void ReceiveChat(string text)
		{
			var msg = new Message<string>("ReceiveChat", text);

			conn.Send(msg.ToBytes());
		}

		public void SetMessage(string msg)
		{
			var message = new Message<string>("SetMessage", msg);

			conn.Send(message.ToBytes());
		}

		public void SetNameOf(int id, string name)
		{
			var msg = new Message<Tuple<int, string>>("SetNameOf", new Tuple<int, string>(id, name));

			conn.Send(msg.ToBytes());
		}

		public void SetPlayerCard(int id, CardInfo card)
		{
			var msg = new Message<Tuple<int, int[]>>("SetPlayerCard", new Tuple<int, int[]>(id, card.ToArray()));

			conn.Send(msg.ToBytes());
		}

		public void ClearZones()
		{
			// There is no need for a argument here,
			// just using boolean so it can run
			var msg = new Message<bool>("ClearZones", false);

			conn.Send(msg.ToBytes());
		}

		public void SetJoker(CardInfo c)
		{
			var msg = new Message<int[]>("SetJoker", c.ToArray());

			conn.Send(msg.ToBytes());
		}

		public void GiveTurnTo(int id)
		{
			var msg = new Message<int>("GiveTurnTo", id);

			conn.Send(msg.ToBytes());
		}

		private IEnumerable<BiscaConnection> connections()
		{
			return parent.Connections.Where(c => c != this);
		}

		public void AddCard(CardInfo info)
		{
			var msg = new Message<int[]>("AddCard", info.ToArray());
			conn.Send(msg.ToBytes());

			foreach (var c in connections())
			{
				c.AddCardTo(this.Index);
			}
		}

		// Add a face down card to player <id>
		public void AddCardTo(int id)
		{
			var msg = new Message<int>("AddCardTo", id);

			conn.Send(msg.ToBytes());
		}

		public void RemoveAt(int idx)
		{
			var msg = new Message<int>("RemoveAt", idx);

			conn.Send(msg.ToBytes());

			foreach (var c in connections())
			{
				c.RemoveFrom(this.Index);
			}
		}

		// Remove a card from the hand of player <id>
		public void RemoveFrom(int id)
		{
			var msg = new Message<int>("RemoveFrom", id);

			conn.Send(msg.ToBytes());
		}

		public void SetHeader(string header)
		{
			var msg = new Message<string>("SetHeader", header);

			conn.Send(msg.ToBytes());
		}

		public void End()
		{
			// The bool here is not used for anything
			var msg = new Message<bool>("End", false);

			conn.Send(msg.ToBytes());

			conn.Shutdown(SocketShutdown.Both);
			conn.Close();
			conn = null;

			cancellationTokenSource.Cancel();

			parent.Remove(this);
		}

		public string GetName()
		{
			return this.Name;
		}

		public int GetID()
		{
			return this.Index;
		}
	}
}
