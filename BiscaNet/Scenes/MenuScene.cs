using System;
using Microsoft.Xna.Framework;
using Prime;
using Prime.UI;
using BiscaNet.Desktop.Data;
using BiscaNet.Desktop.Networking.Server;

namespace BiscaNet.Desktop.Scenes
{
	public class MenuScene : Scene
	{
		public override void Initialize()
		{
			base.Initialize();

			var panel = this.AddUI(new Panel(new Vector2(400, 400), AnchorPoint.Center));

			panel.AddChild(new Header("Bisca Online"));

			var txt = new TextInput(false);
			txt.PlaceholderText = "Seu nome...";

			var check = new Checkbox("Ser o hospedeiro?", AnchorPoint.Center);

			var bt = new Button("Jogar!", AnchorPoint.BottomCenter);
			bt.OnClick += () =>
			{
				var player = PlayerData.GetInstance();
				player.Name = txt.Value;

				if (check.Checkec)
				{
					this.Game.ActiveScene = new ServerConfigScene();
				}
				else
				{
					this.Game.ActiveScene = new ConnectionConfigScene();
				}
			};

			panel.AddChild(txt);
			panel.AddChild(check);
			panel.AddChild(bt);
		}
	}
}
