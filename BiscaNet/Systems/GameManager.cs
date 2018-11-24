using System;
using BiscaNet.Desktop.Scenes;
using BiscaNet.Desktop.Data;
using System.Collections.Generic;
using Prime;
using System.Linq;

namespace BiscaNet.Desktop.Systems
{
	// Tells who should play, gives cards out,
	// basically our god
	// TODO: Replace this with a sockets based server
	public static class GameManager
	{
		private static GameScene game;

		private static GameState state;

		private static Queue<CardInfo> deck;

		private static List<ManagedPlayer> players;
		private static int currentPlayer;

		private static float elapsedTime;

		private static int startingPlayer;

		public static void Init(GameScene g)
		{
			game = g;

			deck = new Queue<CardInfo>(CardInfo.GenerateDeck());

			state = GameState.Initializing;

			players = new List<ManagedPlayer>();
			currentPlayer = 0;

			startingPlayer = 0;

			elapsedTime = 0;
		}

		public static void AddPlayer(IPlayer p)
		{
			players.Add(new ManagedPlayer(p));
		}


		public static void Update()
		{
			string msg;

			switch (state)
			{
				case GameState.Initializing:
					game.Joker = deck.Dequeue();

					// Give out 2 cards for each player
					foreach (var player in players)
					{
						for (int i = 0; i < 2; i++)
						{
							player.AddCard(deck.Dequeue());
						}
					}
					state = GameState.Distrubution;
					break;
				case GameState.Distrubution:

					int playerIndex = 0;
					if (deck.Count < players.Count)
					{
						msg = players[0].Player.HandCount() + " rodadas restantes!\n";
					}
					else
					{
						msg = (deck.Count / players.Count) + 2 + " rodadas restantes!\n";
					}
					foreach (var player in players)
					{
						if (currentPlayer == playerIndex)
						{
							msg += "-> ";
						}
						playerIndex++;

						msg += player.Player.GetName() + ": " + player.Points + " pontos\n";
					}
					game.Message = msg;

					// No more cards to play
					if (players[0].Player.HandCount() <= 0)
					{
						state = GameState.EvaluateWinner;
						break;
					}

					// We need at least 1 card for each player
					if (deck.Count < players.Count)
					{
						state = GameState.Playing;
						break;
					}

					foreach (var player in players)
					{
						player.AddCard(deck.Dequeue());
					}
					state = GameState.Playing;

					break;
				case GameState.Playing:
					var p = players[currentPlayer];
					var cardId = p.Player.PlayCard();

					if (cardId >= 0)
					{
						var c = p.PopCard(cardId);

						game.SetZone((currentPlayer + startingPlayer) % players.Count, c);

						if (++currentPlayer >= players.Count)
						{
							currentPlayer = 0;
							state = GameState.Waiting;
						}
					}
					break;
				case GameState.Waiting:
					elapsedTime += Time.DetlaTime;

					if (elapsedTime > 3)
					{
						elapsedTime = 0;
						state = GameState.Evaluating;
						game.ClearZones();
					}
					break;
				case GameState.Evaluating:
					var bestCard = players[0].OnTable.Value;
					var bestPlayer = players[0];
					int bestIndex = 0;
					int counter = 1;
					foreach (var player in players.Skip(1))
					{
						if (player.OnTable.Value.GreaterThan(bestCard, game.Joker.Suit))
						{
							bestCard = player.OnTable.Value;
							bestPlayer = player;
							bestIndex = counter;
						}
						counter++;
					}

					foreach (var player in players)
					{
						bestPlayer.OnStack.Add(player.OnTable.Value);
						player.OnTable = null;
					}

					startingPlayer += bestIndex;
					startingPlayer = startingPlayer % players.Count;

					var newOrder = players.GetRange(bestIndex, players.Count - bestIndex);
					newOrder.AddRange(players.GetRange(0, bestIndex));
					players = newOrder;

					Console.WriteLine("The best player is " + bestPlayer.Player.GetName());

					state = GameState.Distrubution;
					break;
				case GameState.EvaluateWinner:
					players = players.OrderByDescending((c) => c.Points).ToList();

					msg = "Ranking:\n";
					int rank = 1;
					foreach (var player in players)
					{
						msg += rank + "º: " + player.Player.GetName() + " -> " + player.Points + " pontos\n";
						rank++;
					}

					game.SetHeader(msg);
					elapsedTime = 0;
					state = GameState.Over;
					break;
				case GameState.Over:
					elapsedTime += Time.DetlaTime;
					if (elapsedTime > 3)
					{
						game.End();
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
