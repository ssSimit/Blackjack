using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image cardImage;

    public void SetCard(Card card)
    {
        cardImage.sprite = CardSpriteDatabase.GetSprite(card);
    }
}

