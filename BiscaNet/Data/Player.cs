using System;
using BiscaNet.Desktop.Scenes;
using BiscaNet.Desktop.Systems;
using Prime;
using System.Collections.Generic;
using BiscaNet.Desktop.Entities;
using BiscaNet.Desktop.Networking.Client;
using Microsoft.Xna.Framework.Audio;

namespace BiscaNet.Desktop.Data
{
	/// <summary>
	/// You, the player.
	/// </summary>
	public class Player : Entity, IPlayer
	{
		private GameScene game;
		private BiscaClient client;

		private SoundEffect receiveTurn;

		private List<Card> onHand = new List<Card>();

		public Player(GameScene s, BiscaClient cli)
		{
			this.game = s;
			this.client = cli;
		}

		public override void Initialize()
		{
			base.Initialize();

			receiveTurn = this.Scene.Content.Load<SoundEffect>("Audio/Effects/play_now");
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
		public void SetName(string name)
		{
			throw new Exception("Can't SetName() on the local player!");
		}

		public string GetName()
		{
			return PlayerData.GetInstance().Name;
		}

		public int GetID()
		{
			return PlayerData.GetInstance().Index;
		}

		public int GetZone()
		{
			return 0;
		}

		public void AddCard(CardInfo? info)
		{
			// Add card to scene and hand
			this.onHand.Add(game.Add(new Card(info.Value)));
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
			throw new Exception("Use this player's connection to check which card has been used!");
		}

		public override void Update()
		{
			if (isOurTurn)
			{
				for (int i = 0; i < onHand.Count; i++)
				{
					var coll = onHand[i].Hitbox;

					if (Input.IsButtonPressed(MouseButtons.Left) && coll.CollidesWith(Input.MousePosition(this.game.Cam)))
					{
						client.PlayCard(i);
						break;
					}
				}
			}

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
