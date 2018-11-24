using System;
using System.Linq;
using Prime;

namespace BiscaNet.Desktop.Data
{
	public struct CardInfo
	{
		public static readonly int[] Numbers = { 1, 2, 3, 4, 5, 6, 7, 10, 11, 12 };

		public readonly int Number;
		public readonly Suit Suit;

		public CardInfo(Suit suit, int num)
		{
			this.Suit = suit;
			this.Number = num;

			if (!Numbers.Contains(this.Number))
			{
				throw new ArgumentOutOfRangeException("num", "num must be a valid number for a spanish deck");
			}
		}

		// values[n-1] = value of card n
		private static int[] values = { 0, 0, 0, 0, 10, 0, 0, 0, 0, 2, 3, 4, 0 };
		public int Points
		{
			get
			{
				return values[this.Number - 1];
			}
		}

		public static CardInfo[] GenerateDeck()
		{
			var res = new CardInfo[40];
			int index = 0;

			foreach (var i in CardInfo.Numbers)
			{
				res[index] = new CardInfo(Suit.Bastos, i);
				res[index + 1] = new CardInfo(Suit.Copas, i);
				res[index + 2] = new CardInfo(Suit.Espadas, i);
				res[index + 3] = new CardInfo(Suit.Oros, i);

				index += 4;
			}

			// Shuffle
			return res.OrderBy((c) => Randomic.Rand()).ToArray();
		}

		// Returns true if this wins out against the b card,
		// taking note of the joker suit
		public bool GreaterThan(CardInfo b, Suit joker)
		{
			if (this == b)
			{
				throw new ArgumentException("Can't check values against two equal cards!");
			}

			if (this.Suit == b.Suit)
			{
				// Both cards are worth 0 points,
				// use their number for points
				if (this.Points == b.Points)
				{
					return this.Number > b.Number;
				}
				else
				{
					return this.Points > b.Points;
				}
			}

			// See if someone is the joker
			if (this.Suit == joker) return true;
			if (b.Suit == joker) return false;

			// Last resource: different suits, and
			// no joker in play
			if (this.Points == b.Points)
			{
				return this.Number > b.Number;
			}
			else
			{
				return this.Points > b.Points;
			}
		}

		public bool Equals(CardInfo b)
		{
			return this.Number == b.Number && this.Suit == b.Suit;
		}

		public static bool operator ==(CardInfo a, CardInfo b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(CardInfo a, CardInfo b)
		{
			return !(a == b);
		}
	}

	public enum Suit
	{
		Bastos,
		Copas,
		Espadas,
		Oros
	}

	public static class SuitExtensions
	{
		public static string Name(this Suit s)
		{
			switch (s)
			{
				case Suit.Bastos:
					return "bastos";
				case Suit.Copas:
					return "copas";
				case Suit.Espadas:
					return "espadas";
				case Suit.Oros:
					return "oros";
			}

			throw new ArgumentOutOfRangeException("Suit s", "s is not in the enum interval");
		}
	}
}
