using System.Collections.Generic;
public class Hand
{
    public List<Card> cards = new List<Card>();

    public int GetHandValue()
    {
        int total = 0;
        int aceCount = 0;

        foreach (Card card in cards)
        {
            total += card.GetBlackjackValue();
            if (card.rank == Rank.Ace)
                aceCount++;
        }

        // Adjust Aces
        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }
}
