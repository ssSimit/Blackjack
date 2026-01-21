using UnityEngine;
using TMPro;

public class BetButton : MonoBehaviour
{
    public int playerIndex;
    [SerializeField] TextMeshProUGUI playerChipsText;
    [SerializeField] TextMeshProUGUI betAmountText;
    [SerializeField] RectTransform buttonTransform;
    ProfileAndBetManager pbm;
    GameManager gm;
    void Start()
    {
        pbm = ProfileAndBetManager.Instance;
        gm = GameManager.Instance;
        gm.nextPlayerTurnEvent.AddListener(PlayerActionButtons);
        PlayerActionButtons();
    }

    public void PlaceBet(int amount)
    {
        pbm.PlaceBet(playerIndex, amount);
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

}
