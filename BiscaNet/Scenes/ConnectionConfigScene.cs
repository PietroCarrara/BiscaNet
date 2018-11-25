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

			panel.AddChild(new Label("Endereço de ip do host:"));
			panel.AddChild(txt);

			var bt = new Button("Jogar!");
			bt.OnClick = () =>
			{
				this.Game.ActiveScene = new ConnectionScene(txt.Value);
			};

			panel.AddChild(bt);
		}
	}
}
