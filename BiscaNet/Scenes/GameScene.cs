using System;
using Prime;
using Prime.UI;
using Prime.Graphics;
using Microsoft.Xna.Framework.Graphics;
using BiscaNet.Desktop.Entities;
using BiscaNet.Desktop.Data;
using BiscaNet.Desktop.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BiscaNet.Desktop.Scenes
{
	public class GameScene : Scene
	{
		public readonly Entity DeckEntity = new Entity();

		private List<Card> onTable = new List<Card>();
		private List<Card> onHand = new List<Card>();

		public override void Initialize()
		{
			base.Initialize();

			GameManager.Init(this);

			this.AddUI(new Label("Tabela de pontuação:\n 5: 10 pontos\n12: 4 pontos\n11: 3 pontos\n10: 2 pontos", AnchorPoint.BottomLeft));

			var bg = this.Add(new Entity());
			bg.Add(new Sprite(this.Content.Load<Texture2D>("Sprites/bg/table-top")));

			// Setup face-down deck
			this.Add(DeckEntity);
			DeckEntity.Position = PrimeGame.Center + new Vector2(200, 0);
			var deckSpr = DeckEntity.Add(new Sprite(Content.Load<Texture2D>("Sprites/cards/card-back")));
			deckSpr.Width = Card.Width;
			deckSpr.Height = Card.Height;
		}

		public override void Update()
		{
			base.Update();

			GameManager.Update();

			for (int i = 0; i < onHand.Count; i++)
			{
				var c = onHand[i];

				var coll = c.GetComponent<RectangleCollider>();

				// The hover effect
				if (coll.CollidesWith(Input.MousePosition(this.Cam)))
				{
					c.Position.Y = 720 - Card.Height / 2;
				}
				else
				{
					c.Position.Y = 720 - Card.Height / 4;
				}

				// Playing a card
				if (Input.IsButtonPressed(MouseButtons.Left) && coll.CollidesWith(Input.MousePosition(this.Cam)))
				{
					AddToTable(c.Info);
					RemoveFromHand(c.Info);
					i--;

					// If we don't stop, the cards to the right
					// are gonna come and collide with the cursor (and be played).
					// We don't want that to happen, so afer we have player one, stop looking to the others.
					break;
				}
			}
		}

		public void AddToTable(CardInfo c)
		{
			var card = this.Add(new Card(c));
			onTable.Add(card);
			card.Position.Y = PrimeGame.Center.Y;
			card.Position.X = PrimeGame.Center.X / 2 + (Card.Width / 2) * onTable.Count;
		}

		public void AddToHand(CardInfo c)
		{
			var card = this.Add(new Card(c));
			onHand.Add(card);
			card.Position.Y = 720 - Card.Height / 4;
			card.Position.X = PrimeGame.Center.X * 2 / 3 + (Card.Width) * onHand.Count;

			card.Add(new RectangleCollider(Card.Width, Card.Height));
		}

		public void RemoveFromHand(CardInfo c)
		{
			for (int i = 0; i < onHand.Count; i++)
			{
				if (onHand[i].Info == c)
				{
					onHand[i].Destroy();
					onHand.RemoveAt(i);
					break;
				}
			}

			int index = 1;
			foreach (var card in onHand)
			{
				card.Position.X = PrimeGame.Center.X * 2 / 3 + (Card.Width) * index;
				index++;
			}
		}

		public void DestroyDeck()
		{
			DeckEntity.Destroy();
		}

	}
}
