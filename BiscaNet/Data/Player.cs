using System;
using BiscaNet.Desktop.Scenes;
using BiscaNet.Desktop.Systems;
using Prime;
using System.Collections.Generic;
using BiscaNet.Desktop.Entities;

namespace BiscaNet.Desktop.Data
{
	/// <summary>
	/// You, the player.
	/// </summary>
	public class Player : Entity, IPlayer
	{
		private GameScene game;

		private List<Card> onHand = new List<Card>();

		public Player(GameScene s)
		{
			this.game = s;
		}

		public string GetName()
		{
			return PlayerData.GetInstance().Name;
		}

		public void AddCard(CardInfo info)
		{
			// Add card to scene and hand
			this.onHand.Add(game.Add(new Card(info)));
		}

		public void RemoveAt(int idx)
		{
			onHand[idx].Destroy();
			onHand.RemoveAt(idx);
		}

		public int HandCount()
		{
			return onHand.Count;
		}

		public int PlayCard()
		{
			for (int i = 0; i < onHand.Count; i++)
			{
				var coll = onHand[i].Hitbox;

				if (Input.IsButtonPressed(MouseButtons.Left) && coll.CollidesWith(Input.MousePosition(this.game.Cam)))
				{
					return i;
				}
			}

			return -1;
		}

		public override void Update()
		{
			for (int i = 0; i < onHand.Count; i++)
			{
				var c = onHand[i];

				c.Position.X = PrimeGame.Center.X + (Card.Width) * (i - 1);

				var coll = c.Hitbox;

				// The hover effect
				if (coll.CollidesWith(Input.MousePosition(game.Cam)))
				{
					c.Position.Y = 720 - Card.Height / 2;
				}
				else
				{
					c.Position.Y = 720 - Card.Height / 4;
				}
			}
		}
	}
}
