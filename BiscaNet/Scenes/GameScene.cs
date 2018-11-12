using System;
using Prime;
using Prime.UI;
using Prime.Graphics;
using Microsoft.Xna.Framework.Graphics;
using BiscaNet.Desktop.Entities;
using BiscaNet.Desktop.Data;
namespace BiscaNet.Desktop.Scenes
{
	public class GameScene : Scene
	{
		public GameScene()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.AddUI(new Label("Tabela de pontuação:\n 5: 10 pontos\n12: 4 pontos\n11: 3 pontos\n10: 2 pontos", AnchorPoint.BottomLeft));

			var bg = this.Add(new Entity());
			bg.Add(new Sprite(this.Content.Load<Texture2D>("Sprites/bg/table-top")));

			var c = this.Add(new Card(new CardInfo(Suit.Copas, 4)));
			c.Position = PrimeGame.Center;
		}

	}
}
