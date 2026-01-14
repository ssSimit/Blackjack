using System.Collections.Generic;
using UnityEngine;
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

    public void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);
            (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
        }
    }

    public Card DrawCard()
    {
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }


}
