using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using BiscaNet.Desktop.Networking.Protocol;
using System.Linq;
using BiscaNet.Desktop.Systems;
using BiscaNet.Desktop.Data;
using System.Threading;

namespace BiscaNet.Desktop.Networking.Server
{
	public class BiscaServer
	{
		// Number of players in the game
		private int numPlayers;

		public List<BiscaConnection> Connections = new List<BiscaConnection>();

		private Socket conn;

		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		private string msg;
		public string Message
		{
			get => msg;
			set
			{
				if (msg != value)
				{
					msg = value;
					foreach (var c in Connections)
					{
						c.SetMessage(msg);
					}
				}
			}
		}

		public bool Running
		{
			get => conn != null;
		}

		public BiscaServer(int numPlayers)
		{
			this.numPlayers = numPlayers;
		}

		int connectionsRemoved = 0;
		public void Remove(BiscaConnection co)
		{
			connectionsRemoved++;

			// If empty, stop
			if (connectionsRemoved >= Connections.Count)
			{
				this.end();
			}
		}

		public void Start()
		{
			GameManager.ShouldRun = true;
			GameManager.Server = this;

			conn = new Socket(SocketType.Stream, ProtocolType.Tcp);
			conn.Bind(new IPEndPoint(IPAddress.Any, Values.GamePort));
			conn.Listen(6);

			Task.Run(() =>
			{
				try
				{
					watch();
				}
				catch (Exception e)
				{
					Console.WriteLine("BiscaServer: Error while wating for connections!: " + e.Message);
					Console.WriteLine("BiscaServer: Closing connection...");
					// TODO: Close connection
				}
			}, cancellationTokenSource.Token);
		}

		private void end()
		{
			conn.Close();

			conn = null;

			cancellationTokenSource.Cancel();
		}

		private CardInfo joker;
		public void SetJoker(CardInfo c)
		{
			joker = c;
			foreach (var co in Connections)
			{
				co.SetJoker(c);
			}
		}
		public CardInfo GetJoker()
		{
			return joker;
		}

		public void SetHeader(string msg)
		{
			foreach (var co in Connections)
			{
				co.SetHeader(msg);
			}
		}

		public void SetZone(int playerID, CardInfo c)
		{
			foreach (var co in Connections)
			{
				co.SetPlayerCard(playerID, c);
			}
		}

		public void ClearZones()
		{
			foreach (var co in Connections)
			{
				co.ClearZones();
			}
		}

		public void GiveTurnTo(int id)
		{
			foreach (var co in Connections)
			{
				co.GiveTurnTo(id);
			}
		}

		public void SendChat(string player, string message)
		{
			foreach (var co in Connections)
			{
				co.ReceiveChat(player + ": " + message);
			}
		}

		public void End()
		{
			foreach (var co in Connections)
			{
				co.End();
			}
		}

		private void watch()
		{
			while (this.Running)
			{
				Console.WriteLine("BiscaServer: Waiting for a connection...");
				var sock = new BiscaConnection(conn.Accept(), this);

				sock.SetIndex(Connections.Count);

				// Alert everyone of a new player
				foreach (var c in Connections)
				{
					c.AddPlayer(Connections.Count, Connections.Count);
				}

				this.Connections.Add(sock);

				Console.WriteLine("BiscaServer: A client has connected to the server!");

				// The game can start
				if (Connections.Count >= this.numPlayers)
				{
					// Wait for everyone to send their configs...
					foreach (var c in Connections)
					{
						// Wait...
						while (!c.Ready) { }
					}

					// The first second player is missing 1 enemy, the third two...
					// Add remaining players
					int index = 0;
					foreach (var c in Connections)
					{
						for (int i = 0; i < index; i++)
						{
							c.AddPlayer((4 - index + i) % 4, i);
						}
						index++;
					}

					// Send the configs
					foreach (var c in Connections)
					{
						foreach (var co in Connections)
						{
							// Skip ourselves...
							if (c == co) continue;

							c.SetNameOf(co.Index, co.Name);
						}
					}

					foreach (var conn in Connections)
					{
						conn.SetGameReady(true);
					}

					return;
				}
			}
		}
	}
}
