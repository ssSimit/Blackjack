using System.Collections.Generic;
using UnityEngine;
using System;
public static class CardSpriteDatabase
{
    private static Dictionary<string, Sprite> spriteCache;

    public static void Initialize(Sprite[] sprites)
    {
        spriteCache = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites)
        {
            spriteCache[sprite.name.ToLower()] = sprite;
        }
    }
    public static Sprite GetSprite(Card card)
    {
        int suitIndex = GetSuitIndex(card.suit);
        int rankIndex = (int)card.rank - 1;

        int spriteIndex = suitIndex * 13 + rankIndex;
        string key = $"cardsspritesheet_{spriteIndex}";


        return spriteCache[key];
    }

    public static Sprite GetBacksideSprite()
    {
        string key = "cardsspritesheet_52";
        return spriteCache[key];
    }
    private static int GetSuitIndex(Suit suit)
    {
        return suit switch
        {
            Suit.Spades => 0,
            Suit.Clubs => 1,
            Suit.Diamonds => 2,
            Suit.Hearts => 3,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

}
