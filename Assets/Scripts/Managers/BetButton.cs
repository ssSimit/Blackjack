using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class BetButton : MonoBehaviour
{
    public int playerIndex;
    [SerializeField] TextMeshProUGUI playerChipsText;
    [SerializeField] TextMeshProUGUI betAmountText;

    [SerializeField] TextMeshProUGUI feedbackText;

    [SerializeField] RectTransform buttonTransform;
    [SerializeField] RectTransform placedBetPosition;
    [SerializeField] Image avatarImage;
    ProfileAndBetManager pbm;
    GameManager gm;
    void Start()
    {
        pbm = ProfileAndBetManager.Instance;
        gm = GameManager.Instance;
        gm.nextPlayerTurnEvent.AddListener(PlayerActionButtons);
        gm.sendPlayerFeedback.AddListener((message, index, color) =>
        {
            if (index == playerIndex)
            {
                feedbackText.text = message;
                feedbackText.color = color;
            }
        });
        gm.roundResolved.AddListener(UpdateChipText);
        PlayerActionButtons();
    }

    public void PlaceBet(int amount)
    {
        pbm.PlaceBet(playerIndex, amount, placedBetPosition, playerChipsText.GetComponent<RectTransform>());
        playerChipsText.text = pbm.totalPlayerChips[playerIndex].ToString();
        betAmountText.text = amount.ToString();
    }

    void PlayerActionButtons()
    {
        if (gm.currentPlayerIndex == playerIndex)
        {
            pbm.SetPlayerActionButtons(buttonTransform);
        }
    }

    void UpdateChipText()
    {
        playerChipsText.text = pbm.totalPlayerChips[playerIndex].ToString();
    }

}
