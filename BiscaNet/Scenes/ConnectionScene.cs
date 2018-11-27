using System;
using Prime;
using Prime.UI;
using BiscaNet.Desktop.Networking.Client;
using BiscaNet.Desktop.Networking.Protocol;
using Microsoft.Xna.Framework;

namespace BiscaNet.Desktop.Scenes
{
	public class ConnectionScene : Scene
	{
		private BiscaClient client;

		private GameScene game;

		// Have any connection errors been handled?
		private bool handledError;

		protected Label Label;

		private Scene parent;

		private int prevPlayers;

		private string addr;

		public new bool Initialized { get; private set; }

		public ConnectionScene(string addr, Scene parent)
		{
			this.addr = addr;
			this.parent = parent;
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Initialized = true;

			this.Label = this.AddUI(new Label("Conectando ao servidor...", AnchorPoint.Center));

			Prepare();
		}

		protected virtual string GetText()
		{
			return "Conectando-se ao jogo...";
		}

		public virtual void Prepare()
		{
			this.Label.Text = this.GetText();

			this.client = new BiscaClient(addr, Values.GamePort);
			this.game = new GameScene(client, this.parent);
			this.client.Game = game;

			this.client.Start();
		}

		public override void Update()
		{
			base.Update();

			if (this.client.HasError && !handledError)
			{
				var bt = this.AddUI(new Button("Voltar", AnchorPoint.TopLeft, new Vector2(200, 50)));
				bt.OnClick = () =>
				{
					this.Game.ActiveScene = this.parent;
				};

				this.Label.Text = "Whoops! Algo deu errado!";

				handledError = true;
			}

			if (client.NumberOfPlayers != prevPlayers)
			{
				prevPlayers = client.NumberOfPlayers;

				this.Label.Text = this.GetText() + "\nJogadores conectados: " + prevPlayers;
			}

			if (this.client.GameReady)
			{
				this.Game.ActiveScene = game;
			}
		}
	}
}
