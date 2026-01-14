
using System.Collections.Generic;
using UnityEngine;
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
        string key = $"{card.suit.ToString().ToLower()}_{card.rank.ToString().ToLower()}";
        return spriteCache[key];
    }
}
