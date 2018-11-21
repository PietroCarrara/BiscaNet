using System;
using BiscaNet.Desktop.Data;
using System.Collections.Generic;

namespace BiscaNet.Desktop.Systems
{
	// A player as seen by the game manager.
	// Because an IPlayer doesn't know much about itself
	// (like what cards are in it's hand), we use this to
	// keep track of that info.
	public class ManagedPlayer
	{
		public readonly IPlayer Player;

		public List<CardInfo> OnHand = new List<CardInfo>();

		public CardInfo? OnTable;

		public ManagedPlayer(IPlayer p)
		{
			this.Player = p;
		}

		public void AddCard(CardInfo c)
		{
			OnHand.Add(c);
			Player.AddCard(c);
		}

		public CardInfo PopCard(int idx)
		{
			var c = OnHand[idx];
			OnHand.RemoveAt(idx);

			Player.RemoveAt(idx);

			this.OnTable = c;

			return c;
		}
	}
}
