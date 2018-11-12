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
		}

		public void AddToTable(CardInfo c)
		{
			var card = this.Add(new Card(c));
			onTable.Add(card);
			card.Position.Y = PrimeGame.Center.Y;
			card.Position.X = PrimeGame.Center.X / 2 + (Card.Width / 2) * onTable.Count;
		}

		public void DestroyDeck()
		{
			DeckEntity.Destroy();
		}

	}
}
