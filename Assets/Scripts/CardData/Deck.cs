using System.Collections.Generic;
public class Deck
{
    private List<Card> cards = new List<Card>();

    public void InitializeDeck()
    {
        cards.Clear();

        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                cards.Add(new Card { suit = suit, rank = rank });
            }
        }
    }
}
