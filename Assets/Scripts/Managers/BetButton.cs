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
    [SerializeField] GameObject betChipGO;
    [SerializeField] GameObject betOptionGO;
    [SerializeField] GameObject[] allBetBtns;
    [SerializeField] GameObject refillBtn;

    [SerializeField] Image avatarImage;

    int[] betAmounts = new int[] { 20, 50, 100, 1000 };
    int totalEnabledBets = 0;
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
        gm.roundResolved.AddListener(RoundResolveUpdate);
        pbm.doubleBetEvent.AddListener((index, newBet) =>
        {
            if (index == playerIndex)
            {
                UpdateBetAmountTexts(newBet);
            }
        });
        PlayerActionButtons();
    }

    public void PlaceBet(int amount)
    {
        pbm.PlaceBet(playerIndex, amount, placedBetPosition, playerChipsText.GetComponent<RectTransform>(), betChipGO);
        UpdateBetAmountTexts(amount);
    }

    void UpdateBetAmountTexts(int amount)
    {
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

    void RoundResolveUpdate()
    {
        betChipGO.SetActive(false);
        playerChipsText.text = pbm.totalPlayerChips[playerIndex].ToString();
        feedbackText.text = "";
        betAmountText.text = "0";
        betOptionGO.SetActive(true);
        CheckBalanceAndEnableBetOptions();
        PlayerActionButtons();
    }

    void CheckBalanceAndEnableBetOptions()
    {
        totalEnabledBets = 0;
        for (int i = 0; i < allBetBtns.Length; i++)
        {
            Button btn = allBetBtns[i].GetComponent<Button>();
            if (pbm.totalPlayerChips[playerIndex] >= betAmounts[i])
            {
                btn.interactable = true;
                totalEnabledBets++;
            }
            else
            {
                btn.interactable = false;
            }
        }
        if (totalEnabledBets == 0)
        {
            refillBtn.SetActive(true);
        }
    }

    public void RefillChips(int amount)
    {
        pbm.totalPlayerChips[playerIndex] += amount;
        playerChipsText.text = pbm.totalPlayerChips[playerIndex].ToString();
        CheckBalanceAndEnableBetOptions();
        refillBtn.SetActive(false);
    }

}
