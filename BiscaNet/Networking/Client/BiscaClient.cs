using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BiscaNet.Desktop.Data;
using BiscaNet.Desktop.Networking.Protocol;
using BiscaNet.Desktop.Scenes;
using BiscaNet.Desktop.Systems;

namespace BiscaNet.Desktop.Networking.Client
{
	public class BiscaClient
	{
		public bool HasError { get; private set; }

		public GameScene Game;

		private string address;
		private int port;

		public int NumberOfPlayers
		{
			get
			{
				if (this.GameReady) return Game.Players.Count;

				return PlayerData.GetInstance().Index + this.Game.Players.Count;
			}
		}

		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		Socket conn;

		public bool GameReady { get; private set; }

		public BiscaClient(string address, int port)
		{
			this.address = address;
			this.port = port;
		}
		public bool Running
		{
			get => conn != null;
		}

		public void Start()
		{
			conn = new Socket(SocketType.Stream, ProtocolType.Tcp);
			conn.ReceiveBufferSize = Protocol.Values.MessageLength * 128;

			Task.Run(() =>
			{
				Console.WriteLine("BiscaClient: Connecting to " + this.address + ":" + this.port);
				try
				{
					conn.Connect(address, port);

					sendInfo();
					watch();
					Debug.WriteLine("BiscaClient: Client stopped.");
				}
				catch (SocketException e)
				{
					Debug.WriteLine("BiscaClient: Error while runnig: " + e.Message);
					this.HasError = true;
				}
				catch (Exception e)
				{
					Debug.WriteLine("BiscaClient: Error while runnig: " + e.Message);
				}
			}, cancellationTokenSource.Token);
		}

		public void PlayCard(int card)
		{
			var msg = new Message<int>("PlayCard", card);

			conn.Send(msg.ToBytes());
		}

		public void SendChat(string text)
		{
			var msg = new Message<string>("SendChat", text);

			conn.Send(msg.ToBytes());
		}

		private void watch()
		{
			while (this.Running)
			{
				if (conn.Available >= Protocol.Values.MessageLength)
				{
					var bytes = new byte[Protocol.Values.MessageLength];
					conn.Receive(bytes);
					var str = Encoding.UTF8.GetString(bytes);

					Console.WriteLine("BiscaClient: message received!: " + str);

					var msg = Message<object>.Parse(str);

					switch (msg.Name)
					{
						case "SetGameReady":
							setGameReady(Convert.ToBoolean(msg.Payload));
							break;
						case "AddPlayer":
							addPlayer(Message<Tuple<int, int>>.Parse(str).Payload);
							break;
						case "SetIndex":
							setIndex(Convert.ToInt32(msg.Payload));
							break;
						case "SetNameOf":
							setNameOf(Message<Tuple<int, string>>.Parse(str).Payload);
							break;
						case "SetPlayerCard":
							setPlayerCard(Message<Tuple<int, int[]>>.Parse(str).Payload);
							break;
						case "AddCard":
							addCard(Message<int[]>.Parse(str).Payload);
							break;
						case "AddCardTo":
							addCardTo(Convert.ToInt32(msg.Payload));
							break;
						case "SetJoker":
							setJoker(Message<int[]>.Parse(str).Payload);
							break;
						case "RemoveAt":
							removeAt(Convert.ToInt32(msg.Payload));
							break;
						case "RemoveFrom":
							removeFrom(Convert.ToInt32(msg.Payload));
							break;
						case "ClearZones":
							clearZones();
							break;
						case "SetMessage":
							string m = Convert.ToString(msg.Payload);
							GameSync.Defer(() => Game.Message = m);
							break;
						case "GiveTurnTo":
							giveTurnTo(Convert.ToInt32(msg.Payload));
							break;
						case "SetHeader":
							string header = Convert.ToString(msg.Payload);
							GameSync.Defer(() => Game.SetHeader(header));
							break;
						case "End":
							GameSync.Defer(() => Game.End());
							end();
							break;
						case "ReceiveChat":
							GameSync.Defer(() => Game.AddChat(Convert.ToString(msg.Payload)));
							break;
						case "SetStartingPlayer":
							int start = Convert.ToInt32(msg.Payload);
							GameSync.Defer(() => Game.StartingPlayer = start);
							break;
					}
				}
			}
		}

		private void setGameReady(bool ready)
		{
			GameReady = ready;
			Console.WriteLine("BiscaClient: Game is ready:" + ready);
		}

		public void addPlayer(Tuple<int, int> t)
		{
			// zone and index
			Game.Players.Add(new LocalEnemyPlayer(t.Item1, t.Item2));
		}

		public void setIndex(int idx)
		{
			PlayerData.GetInstance().Index = idx;
		}

		private void setNameOf(Tuple<int, string> data)
		{
			var id = data.Item1;
			var name = data.Item2;

			var player = getById(id);

			player.SetName(name);
		}

		private void sendInfo()
		{
			var msg = new Message<string>("SetName", PlayerData.GetInstance().Name);

			conn.Send(msg.ToBytes());
		}

		private void setPlayerCard(Tuple<int, int[]> data)
		{
			var id = data.Item1;
			var card = CardInfo.FromArray(data.Item2);

			var player = getById(id);

			GameSync.Defer(() =>
			{
				Game.SetZone(player.GetZone(), card);
			});
		}

		private void addCard(int[] data)
		{
			var card = CardInfo.FromArray(data);
			var we = getById(PlayerData.GetInstance().Index);

			GameSync.Defer(() =>
			{
				we.AddCard(card);
			});

		}

		private void addCardTo(int id)
		{
			var player = getById(id);

			GameSync.Defer(() =>
			{
				player.AddCard(null);
			});
		}

		private void removeAt(int i)
		{
			var we = getById(PlayerData.GetInstance().Index);

			GameSync.Defer(() => we.RemoveAt(i));
		}

		private void removeFrom(int id)
		{
			var player = getById(id);

			GameSync.Defer(() => player.RemoveAt(0));
		}

		private void setJoker(int[] data)
		{
			var card = CardInfo.FromArray(data);

			GameSync.Defer(() => Game.Joker = card);
		}

		private void clearZones()
		{
			GameSync.Defer(() => Game.ClearZones());
		}

		private void giveTurnTo(int id)
		{
			GameSync.Defer(() =>
			{
				foreach (var p in Game.Players)
				{
					p.SetTurn(p.GetID() == id);
				}
			});
		}

		private void end()
		{
			conn.Shutdown(SocketShutdown.Both);
			conn.Close();
			conn = null;
			cancellationTokenSource.Cancel();
		}

		private IPlayer getById(int id)
		{
			return Game.Players.Find(p => p.GetID() == id);
		}
	}
}
