using System;
namespace BiscaNet.Desktop.Data
{
	public interface IPlayer
	{
		// Return the card index on the hand array
		// return -1 if no card has been chosen
		int PlayCard();

		// Return how many cards are in hand right now
		int HandCount();

		// Called every update cycle. Use to make
		// operations involving the game
		void Update();

		// Adds a card to the hand
		void AddCard(CardInfo info);

		// Removes the card from the hand at given index
		void RemoveAt(int idx);

		// The player's name
		string GetName();
	}
}
