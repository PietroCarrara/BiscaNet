using System;
using Prime;

namespace BiscaNet.Desktop.Scenes
{
	public class HostConnectionScene : ConnectionScene
	{
		private string ip;

		public HostConnectionScene(string addr, Scene parent, string ip) : base(addr, parent)
		{
			this.ip = ip;
		}


		public override void Initialize()
		{
			base.Initialize();

			this.Label.Text += "\nSeu ip é:\n" + ip;
		}

		public override void Update()
		{
			base.Update();

		}
	}
}
