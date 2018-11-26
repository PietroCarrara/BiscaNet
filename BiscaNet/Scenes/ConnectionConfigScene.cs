using System;
using BiscaNet.Desktop.Networking.Server;
using Prime;
using Prime.UI;
using Microsoft.Xna.Framework;

namespace BiscaNet.Desktop.Scenes
{
	public class ConnectionConfigScene : Scene
	{
		public override void Initialize()
		{
			base.Initialize();

			var panel = this.AddUI(new Panel(new Vector2(400, 400), AnchorPoint.Center));
			panel.AddChild(new Header("Configuração de conexão"));

			var txt = new TextInput(false);
			txt.PlaceholderText = "localhost";

			panel.AddChild(new Label("Endereço de ip do host:"));
			panel.AddChild(txt);

			var bt = new Button("Jogar!");
			bt.OnClick = () =>
			{
				if (txt.Value == "") txt.Value = "127.0.0.1";

				this.Game.ActiveScene = new ConnectionScene(txt.Value, this);
			};

			panel.AddChild(bt);
		}
	}
}
