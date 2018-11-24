using System;
using BiscaNet.Desktop.Data;
using System.Collections.Generic;
using System.Linq;

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

		// The cards a player gets by winning a round
		public List<CardInfo> OnStack = new List<CardInfo>();

		public int Points
		{
			get => OnStack.Sum((c) => c.Points);
		}

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
