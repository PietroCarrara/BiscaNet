using System;
using Prime;
using Prime.UI;
using BiscaNet.Desktop.Networking.Client;
using BiscaNet.Desktop.Networking.Protocol;

namespace BiscaNet.Desktop.Scenes
{
	public class ConnectionScene : Scene
	{
		private BiscaClient client;

		private GameScene game;

		private string addr;

		public ConnectionScene(string addr)
		{
			this.addr = addr;
		}

		public override void Initialize()
		{
			base.Initialize();

			this.AddUI(new Label("Conectando ao servidor...", AnchorPoint.Center));

			this.client = new BiscaClient(addr, Values.GamePort);
			this.game = new GameScene(client);
			this.client.Game = game;

			this.client.Start();
		}

		public override void Update()
		{
			base.Update();

			if (this.client.GameReady)
			{
				this.Game.ActiveScene = game;
			}
		}
	}
}
