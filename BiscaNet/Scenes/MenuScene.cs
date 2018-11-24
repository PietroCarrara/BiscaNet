using System;
using Microsoft.Xna.Framework;
using Prime;
using Prime.UI;
using BiscaNet.Desktop.Data;
namespace BiscaNet.Desktop.Scenes
{
	public class MenuScene : Scene
	{
		public override void Initialize()
		{
			base.Initialize();

			var panel = this.AddUI(new Panel(new Vector2(400, 300), AnchorPoint.Center));

			var txt = new TextInput(false);
			txt.PlaceholderText = "Seu nome...";

			var bt = new Button("Jogar!");
			bt.OnClick += () =>
			{
				var player = PlayerData.GetInstance();
				player.Name = txt.Value;

				this.Game.ActiveScene = new GameScene();
			};

			panel.AddChild(txt);
			panel.AddChild(bt);
		}
	}
}
