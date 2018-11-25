using System;
using BiscaNet.Desktop.Scenes;
using BiscaNet.Desktop.Data;
using System.Collections.Generic;
using Prime;
using System.Linq;
using BiscaNet.Desktop.Networking.Server;

namespace BiscaNet.Desktop.Systems
{
	// Tells who should play, gives cards out,
	// basically our god
	// TODO: Replace this with a sockets based server
	public static class GameManager
	{
		// Only the server should run the update method. The clients will
		// manipulate this class based on the messages sent by the server
		public static bool ShouldRun;

		// We use the server to communicate
		public static BiscaServer Server;

		// private static GameScene game;

		private static GameState state;

		private static Queue<CardInfo> deck;

		public static List<ManagedPlayer> Players;
		private static int currentPlayer;

		private static float elapsedTime;

		private static int startingPlayer;

		public static void Init(GameScene g)
		{
			deck = new Queue<CardInfo>(CardInfo.GenerateDeck());

			state = GameState.Initializing;

			Players = new List<ManagedPlayer>();
			currentPlayer = 0;

			startingPlayer = 0;

			elapsedTime = 0;
		}

		public static void AddPlayer(IPlayer p)
		{
			Players.Add(new ManagedPlayer(p));

			Players = Players.OrderBy(pl => pl.Player.GetID()).ToList();
		}

		public static void Update()
		{
			// Only the server will have this set to true
			if (!ShouldRun) return;

			string msg;

			switch (state)
			{
				case GameState.Initializing:
					Server.SetJoker(deck.Dequeue());

					// Give out 2 cards for each player
					foreach (var player in Players)
					{
						for (int i = 0; i < 2; i++)
						{
							var card = deck.Dequeue();

							player.AddCard(card);
						}
					}
					state = GameState.Distrubution;
					break;
				case GameState.Distrubution:
					if (deck.Count < Players.Count)
					{
						msg = Players[0].HandCount() + " rodadas restantes!\n";
					}
					else
					{
						msg = (deck.Count / Players.Count) + 2 + " rodadas restantes!\n";
					}
					int index = 0;
					foreach (var player in Players)
					{
						if (index == currentPlayer)
						{
							msg += "-> ";
						}
						index++;

						msg += player.Player.GetName() + ": " + player.Points + " pontos\n";
					}
					Server.Message = msg;

					// No more cards to play
					if (Players[0].HandCount() <= 0)
					{
						state = GameState.EvaluateWinner;
						break;
					}

					// We need at least 1 card for each player
					if (deck.Count < Players.Count)
					{
						state = GameState.Playing;
						Server.GiveTurnTo(Players[currentPlayer].Player.GetID());
						break;
					}

					foreach (var player in Players)
					{
						var card = deck.Dequeue();
						player.AddCard(card);
					}
					state = GameState.Playing;
					Server.GiveTurnTo(Players[currentPlayer].Player.GetID());

					break;
				case GameState.Playing:
					var p = Players[currentPlayer];
					var cardId = p.Connection.PlayCard();

					if (cardId >= 0)
					{
						var c = p.PopCard(cardId);

						Server.SetZone(Players[currentPlayer].Player.GetID(), c);


						if (++currentPlayer >= Players.Count)
						{
							currentPlayer = 0;
							state = GameState.Waiting;
						}
						else
						{
							Server.GiveTurnTo(Players[currentPlayer].Player.GetID());
						}
					}
					break;
				case GameState.Waiting:
					elapsedTime += Time.DetlaTime;

					if (elapsedTime > 1)
					{
						elapsedTime = 0;
						state = GameState.Evaluating;
						Server.ClearZones();
					}
					break;
				case GameState.Evaluating:
					var bestCard = Players[0].OnTable.Value;
					var bestPlayer = Players[0];
					int bestIndex = 0;
					int counter = 1;
					foreach (var player in Players.Skip(1))
					{
						if (player.OnTable.Value.GreaterThan(bestCard, Server.GetJoker().Suit))
						{
							bestCard = player.OnTable.Value;
							bestPlayer = player;
							bestIndex = counter;
						}
						counter++;
					}

					foreach (var player in Players)
					{
						bestPlayer.OnStack.Add(player.OnTable.Value);
						player.OnTable = null;
					}

					startingPlayer += bestIndex;
					startingPlayer = startingPlayer % Players.Count;

					var newOrder = Players.GetRange(bestIndex, Players.Count - bestIndex);
					newOrder.AddRange(Players.GetRange(0, bestIndex));
					Players = newOrder;

					Console.WriteLine("The best player is " + bestPlayer.Player.GetName());

					state = GameState.Distrubution;
					break;
				case GameState.EvaluateWinner:
					Players = Players.OrderByDescending((c) => c.Points).ToList();

					msg = "Ranking:\n";
					int rank = 1;
					foreach (var player in Players)
					{
						msg += rank + "º: " + player.Player.GetName() + " -> " + player.Points + " pontos\n";
						rank++;
					}

					Server.SetHeader(msg);
					elapsedTime = 0;
					state = GameState.Over;
					break;
				case GameState.Over:
					elapsedTime += Time.DetlaTime;
					if (elapsedTime > 5)
					{
						Server.End();
						state = GameState.None;
					}
					break;
			}
		}
	}

	public enum GameState
	{
		// Right after the deck has been cut
		Initializing,
		// First step, where the cards are given out
		Distrubution,
		// Everyone is puting their cards in the table
		Playing,
		// Waiting just so the players can understand what cards have been played
		Waiting,
		// Calculating who won the round
		Evaluating,
		// Calculating who won the game
		EvaluateWinner,
		// The game has ended
		Over,
		// We can fully stop doing any checks, the game has totally ended
		None
	}
}
