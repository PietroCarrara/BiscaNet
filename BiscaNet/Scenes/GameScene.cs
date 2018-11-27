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
using BiscaNet.Desktop.Networking.Client;
using Microsoft.Xna.Framework.Input;

namespace BiscaNet.Desktop.Scenes
{
	public class GameScene : Scene
	{
		// TODO: Remove
		public readonly Entity WinningIndicator = new Entity();

		public List<IPlayer> Players = new List<IPlayer>();

		private Label label;

		private Panel dialog;
		private Panel chat;

		private BiscaClient client;

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

		private Scene parent;

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

		public GameScene(BiscaClient cli, Scene parent)
		{
			this.client = cli;

			this.parent = parent;

			You = new Player(this, cli);
			Players.Add(You);
		}

		public void AddChat(string text)
		{
			chat.AddChild(new Label(text));
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

		public void End()
		{
			this.Game.ActiveScene = this.parent;
		}

		private Label header;
		public void SetHeader(string msg)
		{
			if (header == null) header = this.AddUI(new Label("", AnchorPoint.TopCenter));

			header.Text = msg;
		}

		public override void Initialize()
		{
			base.Initialize();

			Players = Players.OrderBy(p => p.GetID()).ToList();

			GameManager.Init(this);

			foreach (var player in Players)
			{
				if (player is Entity e)
				{
					this.Add(e);
				}
				GameManager.AddPlayer(player);
			}

			this.label = this.AddUI(new Label("", AnchorPoint.BottomLeft));
			this.Message = "";

			var bg = this.Add(new Entity());
			bg.Add(new Sprite(this.Content.Load<Texture2D>("Sprites/bg/table-top")));
			bg.DrawOrder = -10;

			this.chat = this.AddUI(new Panel(new Vector2(400, 200), AnchorPoint.BottomRight));
			this.chat.Draggable = true;

			this.dialog = new Panel(new Vector2(400, 300), AnchorPoint.Center);
			var txt = new TextInput(true, new Vector2(300, 150));
			var bt = new Button("Enviar", AnchorPoint.Auto);
			bt.OnClick = () =>
			{
				string text = txt.Value;
				txt.Value = "";

				if (text == "") return;

				client.SendChat(text);

				dialog.IsVisible = false;
			};

			this.AddUI(dialog);

			dialog.AddChild(txt);
			dialog.AddChild(bt);

			dialog.IsVisible = false;

			// Setup crown
			WinningIndicator.DrawOrder = 1;
			this.Add(WinningIndicator);
			var deckSpr = WinningIndicator.Add(new Sprite(Content.Load<Texture2D>("Sprites/icons/crown")));
			deckSpr.Width = 50;
			deckSpr.Height = 50;
			deckSpr.IsVisible = false;
		}

		public override void Update()
		{
			GameSync.Update();

			base.Update();

			GameManager.Update();

			if (Input.IsKeyPressed(Keys.Enter))
			{
				dialog.IsVisible = true;
			}
		}
	}
}
