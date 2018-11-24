using System;
using Prime;
using System.Collections.Generic;
using BiscaNet.Desktop.Entities;
using Microsoft.Xna.Framework;

namespace BiscaNet.Desktop.Data
{
	public class LocalEnemyPlayer : Entity, IPlayer
	{
		private List<Card> onHand = new List<Card>();

		// Which direction to stack the cards
		// false is side-by-side,
		// true is going up
		private bool isUp;

		private int index;

		private readonly float rotation;

		public LocalEnemyPlayer(Vector2 position, float rotation, int index)
		{
			this.Position = position;
			this.rotation = rotation;

			this.index = index;

			if (this.rotation > 0)
			{
				isUp = true;
			}
		}

		public string GetName()
		{
			return "Enemy " + this.index;
		}

		public void AddCard(CardInfo info)
		{
			onHand.Add(this.Scene.Add(new Card(info)));
		}

		public int HandCount()
		{
			return onHand.Count;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public int PlayCard()
		{
			if (Input.IsButtonPressed(MouseButtons.Left))
			{
				int i = 0;
				foreach (var c in onHand)
				{
					if (c.Hitbox.CollidesWith(Input.MousePosition(this.Scene.Cam)))
					{
						return i;
					}
					i++;
				}
			}
			return -1;
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