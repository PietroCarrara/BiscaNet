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

		public static List<CardInfo> OnTable { get; private set; }
		public static List<CardInfo> OnHand { get; private set; }

		private static List<ManagedPlayer> players;
		private static int currentPlayer;

		private static float elapsedTime = 0;

		public static void Init(GameScene g)
		{
			game = g;

			deck = new Queue<CardInfo>(CardInfo.GenerateDeck());

			OnTable = new List<CardInfo>();
			OnHand = new List<CardInfo>();

			state = GameState.Initializing;

			players = new List<ManagedPlayer>();
			currentPlayer = 0;
		}

		public static void AddPlayer(IPlayer p)
		{
			players.Add(new ManagedPlayer(p));
		}


		public static void Update()
		{
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
					// We need at least 1 card for each player
					if (deck.Count < players.Count)
					{
						state = GameState.Over;
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

						game.SetZone(currentPlayer, c);

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
					foreach (var player in players.Skip(1))
					{
						if (player.OnTable.Value.GreaterThan(bestCard, game.Joker.Suit))
						{
							bestCard = player.OnTable.Value;
							bestPlayer = player;
						}
					}

					Console.WriteLine("The best player is " + bestPlayer.Player.GetName());

					state = GameState.Distrubution;
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
		// Waiting just so the players can understand what
		// cards have been played
		Waiting,
		// Calculating who won the round
		Evaluating,
		// The game has ended
		Over
	}
}
