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

		public Card(CardInfo i)
		{
			this.Info = i;
		}

		public override void Initialize()
		{
			base.Initialize();

			var spr = this.Add(
				new Sprite(
					this.Scene.Content.Load<Texture2D>(
						"Sprites/cards/fronts/" + this.Info.Suit.Name() + "_" + this.Info.Number)));


			spr.Width = Width;
			spr.Height = Height;
		}
	}
}
