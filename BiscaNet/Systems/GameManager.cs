using System;
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

		private static RectangleCollider deckColl;

		private static Queue<CardInfo> deck;

		public static void Init(GameScene g)
		{
			game = g;

			deck = new Queue<CardInfo>(CardInfo.GenerateDeck().Skip(35));

			var d = g.DeckEntity;
			deckColl = new RectangleCollider(Card.Width, Card.Height);
			d.Add(deckColl);
		}

		public static void Update()
		{
			if (deck.Any())
			{
				if (Input.IsButtonPressed(MouseButtons.Left) && deckColl.CollidesWith(Input.MousePosition(game.Cam)))
				{
					game.AddToTable(deck.Dequeue());

					if (!deck.Any())
					{
						game.DestroyDeck();
					}
				}
			}

		}
	}
}
