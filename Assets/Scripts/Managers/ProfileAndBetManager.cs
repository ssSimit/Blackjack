using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProfileAndBetManager : MonoBehaviour
{
    public static ProfileAndBetManager Instance;
    private int totalPlayers = 0;
    [SerializeField] private Sprite[] profilePictures;
    [SerializeField] GameObject startGameButton;
    [SerializeField] Button[] addPlayerButtons;
    [SerializeField] GameObject[] betChipsGOs;

    [SerializeField] GameObject actionButtonsGO;
    public int[] playerBets = new int[] { 0, 0, 0 };
    public int[] totalPlayerChips = new int[] { 5000, 5000, 5000 };

    int betNumber = 0;
    GameManager gm;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        gm = GameManager.Instance;
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

    public void PlaceBet(int playerIndex, int amount)
    {
        playerBets[playerIndex] = amount;
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

    public void StartGame()
    {
        gm.StartGame();
        actionButtonsGO.SetActive(true);
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

}
