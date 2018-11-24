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
using System.Linq;

namespace BiscaNet.Desktop.Scenes
{
	public class GameScene : Scene
	{
		public readonly Entity DeckEntity = new Entity();

		private Player You;

		private Card joker;
		public CardInfo Joker
		{
			get => joker.Info;
			set
			{
				if (joker != null) joker.Destroy();
				joker = new Card(value);

				this.Add(joker);

				joker.Rotation = MathHelper.PiOver2;
				joker.Position = PrimeGame.Center;
			}
		}

		// Maximum of 4 players, so only 4 cards will be at the table
		private Card[] cardsOnTable = new Card[4];
		// The position of each zone
		private Vector2[] zonePositions =
		{
			PrimeGame.Center + new Vector2(0, 150),
			PrimeGame.Center + new Vector2(400, 0),
			PrimeGame.Center + new Vector2(0, -200),
			PrimeGame.Center + new Vector2(-400, 0),
		};

		private float[] zoneRotations =
		{
			0,
			MathHelper.PiOver2 + MathHelper.Pi,
			MathHelper.Pi,
			MathHelper.PiOver2,
		};

		public void SetZone(int zone, CardInfo c)
		{
			if (cardsOnTable[zone] != null) cardsOnTable[zone].Destroy();

			cardsOnTable[zone] = this.Add(new Card(c));
			cardsOnTable[zone].DrawOrder = -1;
			cardsOnTable[zone].Position = zonePositions[zone];
			cardsOnTable[zone].Rotation = zoneRotations[zone];
		}

		public void ClearZones()
		{
			for (int i = 0; i < cardsOnTable.Length; i++)
			{
				if (cardsOnTable[i] != null)
				{
					cardsOnTable[i].Destroy();
					cardsOnTable[i] = null;
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			this.You = this.Add(new Player(this));

			GameManager.Init(this);

			GameManager.AddPlayer(You);
			GameManager.AddPlayer(this.Add(new LocalEnemyPlayer(new Vector2(1280 - Card.Height / 3, 0), MathHelper.PiOver2 + MathHelper.Pi, 1)));
			GameManager.AddPlayer(this.Add(new LocalEnemyPlayer(new Vector2(0, 0 - Card.Height / 3), 0, 2)));
			GameManager.AddPlayer(this.Add(new LocalEnemyPlayer(new Vector2(0 + Card.Height / 3, 0), MathHelper.PiOver2, 3)));

			this.AddUI(new Label("Tabela de pontuação:\n 5: 10 pontos\n12: 4 pontos\n11: 3 pontos\n10: 2 pontos", AnchorPoint.BottomLeft));

			var bg = this.Add(new Entity());
			bg.Add(new Sprite(this.Content.Load<Texture2D>("Sprites/bg/table-top")));
			bg.DrawOrder = -10;

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

		// TODO: Maybe remove?
		public void DestroyDeck()
		{
			DeckEntity.Destroy();
		}
	}
}
