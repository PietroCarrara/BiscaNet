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

		protected override string GetText()
		{
			return base.GetText() + "\nSeu ip é:\n" + ip;
		}
	}
}
