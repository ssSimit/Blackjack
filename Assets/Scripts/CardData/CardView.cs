using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image cardImage;

    void Start()
    {
        GameManager.Instance.roundResolved.AddListener(() => Destroy(gameObject));
    }

    public void SetCard(Card card)
    {
        cardImage.sprite = CardSpriteDatabase.GetSprite(card);
    }

    public void SetBackside()
    {
        cardImage.sprite = CardSpriteDatabase.GetBacksideSprite();
    }

}

