public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public enum Rank
{
    Ace = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}

[System.Serializable]
public class Card
{
    public Suit suit;
    public Rank rank;

    public int GetBlackjackValue()
    {
        if (rank == Rank.Jack || rank == Rank.Queen || rank == Rank.King)
            return 10;

        if (rank == Rank.Ace)
            return 11; // Handle Ace as 1 or 11 in hand logic

        return (int)rank;
    }
}

