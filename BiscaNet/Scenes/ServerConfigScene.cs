using System;
using BiscaNet.Desktop.Networking.Server;
using Prime;
using Prime.UI;
using Microsoft.Xna.Framework;
using System.Net;
using System.Net.Sockets;

namespace BiscaNet.Desktop.Scenes
{
	public class ServerConfigScene : Scene
	{
		private Scene parent;
		public ServerConfigScene(Scene parent)
		{
			this.parent = parent;
		}

		public override void Initialize()
		{
			base.Initialize();

			var panel = this.AddUI(new Panel(new Vector2(700, 400), AnchorPoint.Center));
			panel.AddChild(new Header("Configuração de Hospedeiro"));

			var txt = new TextInput(false);
			txt.PlaceholderText = "Número de jogadores (máx. 4)";

			string ipAddress = "";

			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6)
				{
					ipAddress += "\n" + ip.ToString();
				}
			}

			var bt = new Button("Iniciar", AnchorPoint.BottomCenter);
			bt.OnClick = () =>
			{
				int numPlayers = 0;
				int.TryParse(txt.Value, out numPlayers);

				if (numPlayers <= 1 || numPlayers > 4) return;

				var server = new BiscaServer(numPlayers);
				server.Start();

				var scene = new HostConnectionScene("127.0.0.1", this, ipAddress);
				if (scene.Initialized)
				{
					scene.Prepare();
				}
				this.Game.ActiveScene = scene;

			};

			panel.AddChild(txt);

			panel.AddChild(new Label("Seu endereço de IP é:"));
			panel.AddChild(new Header(ipAddress));

			panel.AddChild(bt);

			this.AddUI(new Button("<-", AnchorPoint.TopLeft, new Vector2(150, 50))).OnClick = () =>
			 {
				 this.Game.ActiveScene = this.parent;
			 };

		}
	}
}
