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

			return res.OrderBy((c) => Randomic.Rand()).ToArray();
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
