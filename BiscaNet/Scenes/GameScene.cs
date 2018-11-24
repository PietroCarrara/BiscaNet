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
		// TODO: Remove
		public readonly Entity WinningIndicator = new Entity();

		private Label label;

		private string message;
		public string Message
		{
			get
			{
				return message;
			}
			set
			{
				message = value;

				label.Text = message + "\nTabela de pontuação:\n 5: 10 pontos\n12: 4 pontos\n11: 3 pontos\n10: 2 pontos";
			}
		}

		public int StartingPlayer = 0;

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

			Card bestCard = null;
			int bestIndex = 0;
			for (int i = 0; i < StartingPlayer; i++)
			{
				if (bestCard == null)
				{
					bestCard = cardsOnTable[i];
					bestIndex = i;
					continue;
				}
				else if (cardsOnTable[i] == null)
				{
					continue;
				}
				else if (cardsOnTable[i].Info.GreaterThan(bestCard.Info, Joker.Suit))
				{
					bestCard = cardsOnTable[i];
					bestIndex = i;
				}
			}
			for (int i = StartingPlayer; i < cardsOnTable.Length; i++)
			{
				if (bestCard == null)
				{
					bestCard = cardsOnTable[i];
					bestIndex = i;
					continue;
				}
				else if (cardsOnTable[i] == null)
				{
					continue;
				}
				else if (cardsOnTable[i].Info.GreaterThan(bestCard.Info, Joker.Suit))
				{
					bestCard = cardsOnTable[i];
					bestIndex = i;
				}
			}

			WinningIndicator.Position = (PrimeGame.Center + zonePositions[bestIndex]) / 2;
			WinningIndicator.GetComponent<Sprite>().IsVisible = true;
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

			WinningIndicator.GetComponent<Sprite>().IsVisible = false;
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

			this.label = this.AddUI(new Label("", AnchorPoint.BottomLeft));
			this.Message = "Oloko";

			var bg = this.Add(new Entity());
			bg.Add(new Sprite(this.Content.Load<Texture2D>("Sprites/bg/table-top")));
			bg.DrawOrder = -10;

			// Setup face-down deck
			WinningIndicator.DrawOrder = 1;
			this.Add(WinningIndicator);
			var deckSpr = WinningIndicator.Add(new Sprite(Content.Load<Texture2D>("Sprites/icons/crown")));
			deckSpr.Width = 50;
			deckSpr.Height = 50;
			deckSpr.IsVisible = false;
		}

		public override void Update()
		{
			base.Update();

			GameManager.Update();
		}
	}
}
