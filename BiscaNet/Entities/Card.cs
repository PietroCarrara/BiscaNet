using System;
using Prime;
using System.Linq;
using Prime.Graphics;
using Microsoft.Xna.Framework.Graphics;
using BiscaNet.Desktop.Data;

namespace BiscaNet.Desktop.Entities
{
	public class Card : Entity
	{
		public const float Width = 100, Height = Width * 1.5f;

		public readonly CardInfo Info;

		public readonly RectangleCollider Hitbox = new RectangleCollider(Card.Width, Card.Height);

		public Sprite Sprite { get; private set; }

		private bool isFaceDown;

		private float rotation;
		public float Rotation
		{
			get => rotation;
			set
			{
				rotation = value;

				this.Sprite.Rotation = rotation;
			}
		}

		// Create a face down card
		public Card()
		{
			this.isFaceDown = true;
		}

		public Card(CardInfo i)
		{
			this.Info = i;
		}

		public override void Initialize()
		{
			base.Initialize();

			if (this.isFaceDown)
			{
				this.Sprite = this.Add(new Sprite(this.Scene.Content.Load<Texture2D>("Sprites/cards/card-back")));
			}
			else
			{
				this.Sprite = this.Add(
					new Sprite(
						this.Scene.Content.Load<Texture2D>(
							"Sprites/cards/fronts/" + this.Info.Suit.Name() + "_" + this.Info.Number)));
			}


			Sprite.Width = Width;
			Sprite.Height = Height;

			this.Add(Hitbox);
		}
	}
}
