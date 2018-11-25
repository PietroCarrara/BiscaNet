using System;
using Prime;
using System.Collections.Generic;
using BiscaNet.Desktop.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BiscaNet.Desktop.Data
{
	public class LocalEnemyPlayer : Entity, IPlayer
	{
		private List<Card> onHand = new List<Card>();

		private SoundEffect receiveTurn;

		// Which direction to stack the cards
		// false is side-by-side,
		// true is going up
		private bool isUp;

		private int zone;
		private int index;

		// Set this to indicate what card will be played
		public int CardPlayed = -1;

		private string name;

		private readonly float rotation;

		public LocalEnemyPlayer(int zone, int index)
		{
			switch (zone)
			{
				case 1:
					this.Position = new Vector2(1280 - Card.Height / 3, 0);
					this.rotation = MathHelper.PiOver2 + MathHelper.Pi;
					break;
				case 2:
					this.Position = new Vector2(0, 0 - Card.Height / 3);
					this.rotation = 0;
					break;
				case 3:
					this.Position = new Vector2(0 + Card.Height / 3, 0);
					this.rotation = MathHelper.PiOver2;
					break;
				default:
					throw new ArgumentOutOfRangeException("index", "index must be between 1 and 3!");
			}

			this.zone = zone;
			this.index = index;

			this.name = "Enemy " + this.index;

			if (this.rotation > 0)
			{
				isUp = true;
			}
		}

		private bool isOurTurn = false;
		public void SetTurn(bool isYou)
		{
			isOurTurn = isYou;

			if (isOurTurn)
			{
				receiveTurn.Play();
			}
		}

		public int GetID()
		{
			return this.index;
		}

		public int GetZone()
		{
			return this.zone;
		}

		public void SetName(string name)
		{
			this.name = name;
		}
		public string GetName()
		{
			return this.name;
		}

		public void AddCard(CardInfo? info)
		{
			if (info.HasValue) throw new Exception("Don't give cards to enemies!");

			onHand.Add(this.Scene.Add(new Card()));
		}

		public int HandCount()
		{
			return onHand.Count;
		}

		public override void Initialize()
		{
			base.Initialize();

			this.receiveTurn = this.Scene.Content.Load<SoundEffect>("Audio/Effects/receive_turn");
		}

		public int PlayCard()
		{
			if (!isOurTurn) return -1;

			var res = CardPlayed;
			CardPlayed = -1;
			return res;
		}

		public void RemoveAt(int idx)
		{
			onHand[idx].Destroy();
			onHand.RemoveAt(idx);
		}

		public override void Update()
		{
			base.Update();

			for (int i = 0; i < onHand.Count; i++)
			{
				var c = onHand[i];

				if (isUp)
				{
					c.Rotation = this.rotation;
					c.Position.Y = PrimeGame.Center.Y + (Card.Width) * (i - 1);
					c.Position.X = this.Position.X;
				}
				else
				{
					c.Position.X = PrimeGame.Center.X + (Card.Width) * (i - 1);
					c.Rotation = MathHelper.Pi;
				}
			}
		}
	}
}