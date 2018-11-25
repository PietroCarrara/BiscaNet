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
		public override void Initialize()
		{
			base.Initialize();

			var panel = this.AddUI(new Panel(new Vector2(700, 400), AnchorPoint.Center));
			panel.AddChild(new Header("Configuração de Hospedeiro"));

			var txt = new TextInput(false);
			txt.PlaceholderText = "Número de jogadores (máx. 4)";

			string ipAddress = "undefined";

			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6)
				{
					ipAddress = ip.ToString();
					break;
				}
			}

			var bt = new Button("Iniciar", AnchorPoint.BottomCenter);
			bt.OnClick = () =>
			{
				int numPlayers = 0;
				int.TryParse(txt.Value, out numPlayers);

				if (numPlayers <= 0 || numPlayers > 4) return;

				var server = new BiscaServer(2);
				server.Start();

				this.Game.ActiveScene = new ConnectionScene("127.0.0.1");
			};

			panel.AddChild(txt);

			panel.AddChild(new Label("Seu endereço de IP é:"));
			panel.AddChild(new Header(ipAddress));

			panel.AddChild(bt);
		}
	}
}
