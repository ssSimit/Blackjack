using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
public class ProfileAndBetManager : MonoBehaviour
{
    public static ProfileAndBetManager Instance;
    private int totalPlayers = 0;
    [SerializeField] private Sprite[] profilePictures;
    [SerializeField] GameObject startGameButton;
    [SerializeField] Button[] addPlayerButtons;
    [SerializeField] GameObject[] betChipsGOs;

    [SerializeField] GameObject actionButtonsGO;
    [SerializeField] GameObject doubleButtonGO;
    public int[] playerBets = new int[] { 0, 0, 0 };
    [SerializeField] RectTransform[] placedBetPositions;
    [SerializeField] RectTransform[] playerBankPositions;
    [SerializeField] RectTransform dealerBankPosition;

    public int[] totalPlayerChips = new int[] { 5000, 5000, 5000 };

    int betNumber = 0;
    GameManager gm;

    public UnityEvent<int, int> doubleBetEvent = new UnityEvent<int, int>();

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        gm = GameManager.Instance;
        gm.dealersTurn.AddListener(() =>
        {
            actionButtonsGO.SetActive(false);
            doubleButtonGO.SetActive(false);
        });
        gm.sendPlayerWinLoss.AddListener(handleLossAndWin);
        gm.nextPlayerTurnEvent.AddListener(CheckAndEnableDoubleButton);
        gm.playerDoubledDown.AddListener((index) =>
        {
            DoubleBet(index);
            doubleButtonGO.SetActive(false);
        });
    }
    public void AddPlayer(GameObject PlayerHand)
    {
        totalPlayers++;
        gm.playerCount = totalPlayers;
        gm.playerCardAnchors.Add(PlayerHand.GetComponent<RectTransform>());
        gm.playerHandHorizontalGroups.Add(PlayerHand.GetComponent<HorizontalLayoutGroup>());
        gm.playerHandValueTexts.Add(PlayerHand.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
    }

    public void SetProfilePicture(Image profileImage)
    {
        profileImage.sprite = profilePictures[totalPlayers - 1];
    }

    public void SetBetOptions(BetButton bet)
    {
        bet.playerIndex = totalPlayers - 1;
    }

    public void PlaceBet(int playerIndex, int amount, RectTransform placedPosition, RectTransform bankPosition, GameObject betChipsGO)
    {
        playerBets[playerIndex] = amount;
        placedBetPositions[playerIndex] = placedPosition;
        playerBankPositions[playerIndex] = bankPosition;
        betChipsGOs[playerIndex] = betChipsGO;
        totalPlayerChips[playerIndex] -= amount;
        betNumber++;
        if (betNumber >= totalPlayers)
        {
            startGameButton.SetActive(true);
            betNumber = 0;
            foreach (Button btn in addPlayerButtons)
            {
                if (btn.interactable)
                {
                    btn.gameObject.SetActive(false);
                }
            }
        }
    }

    void DoubleBet(int playerIndex)
    {
        int currentBet = playerBets[playerIndex];
        if (totalPlayerChips[playerIndex] >= currentBet)
        {
            playerBets[playerIndex] += currentBet;
            totalPlayerChips[playerIndex] -= currentBet;
            doubleBetEvent.Invoke(playerIndex, playerBets[playerIndex]);

        }
    }

    void CheckAndEnableDoubleButton()
    {
        if (totalPlayerChips[gm.currentPlayerIndex] >= playerBets[gm.currentPlayerIndex])
        {
            doubleButtonGO.SetActive(true);
        }
        else
        {
            doubleButtonGO.SetActive(false);
        }
    }

    public void StartGame()
    {
        actionButtonsGO.SetActive(true);
        CheckAndEnableDoubleButton();
        gm.StartGame();
        SetBetChips();
    }

    public void SetPlayerActionButtons(RectTransform buttonTransform)
    {
        actionButtonsGO.GetComponent<RectTransform>().position = buttonTransform.position;
    }


    void SetBetChips()
    {
        for (int i = 0; i < totalPlayers; i++)
        {
            betChipsGOs[i].SetActive(true);
        }
    }

    void handleLossAndWin(bool won, int index, bool push)
    {
        if (!push)
        {
            if (won)
            {
                float multiplier = 2f;
                if (gm.playersWithBlackjack.Contains(index))
                {
                    multiplier = 2.5f;
                }
                int winnings = (int)(playerBets[index] * multiplier);
                totalPlayerChips[index] += winnings;
                StartCoroutine(ChipShowerManager.Instance.FlyCoinAndCard(dealerBankPosition, playerBankPositions[index]));
            }
            else
            {
                StartCoroutine(ChipShowerManager.Instance.FlyCoinAndCard(placedBetPositions[index], dealerBankPosition));
            }
        }
        else
        {
            totalPlayerChips[index] += playerBets[index];
            StartCoroutine(ChipShowerManager.Instance.FlyCoinAndCard(placedBetPositions[index], playerBankPositions[index]));
        }

        playerBets[index] = 0;
    }


}
