﻿using System;
using BiscaNet.Desktop.Scenes;
using Prime;
using BiscaNet.Desktop.Entities;
using BiscaNet.Desktop.Data;
using System.Collections.Generic;
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

		private static RectangleCollider deckColl;

		private static Queue<CardInfo> deck;

		public static List<CardInfo> OnTable { get; private set; }
		public static List<CardInfo> OnHand { get; private set; }

		private static List<ManagedPlayer> players;
		private static int currentPlayer;

		public static void Init(GameScene g)
		{
			game = g;

			deck = new Queue<CardInfo>(CardInfo.GenerateDeck());

			var d = g.DeckEntity;
			deckColl = new RectangleCollider(Card.Width, Card.Height);
			d.Add(deckColl);

			OnTable = new List<CardInfo>();
			OnHand = new List<CardInfo>();

			state = GameState.Distrubution;

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
				case GameState.Distrubution:
					game.Joker = new Card(deck.Dequeue());
					state = GameState.Playing;
					foreach (var player in players)
					{
						for (int i = 0; i < 3; i++)
							player.AddCard(deck.Dequeue());
					}

					// Testing all the zones
					for (int i = 0; i < 4; i++)
					{
						game.SetZone(i, deck.Dequeue());
					}

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
						}
					}

					break;
			}

			if (deck.Any())
			{
				if (Input.IsButtonPressed(MouseButtons.Left) && deckColl.CollidesWith(Input.MousePosition(game.Cam)))
				{
					players[0].AddCard(deck.Dequeue());

					if (!deck.Any())
					{
						game.DestroyDeck();
					}
				}
			}
		}
	}

	public enum GameState
	{
		// First step, where the cards are given out
		Distrubution,
		// Everyone is puting their cards in the table
		Playing
	}
}