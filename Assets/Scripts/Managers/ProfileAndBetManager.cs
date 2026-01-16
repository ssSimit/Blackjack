using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProfileAndBetManager : MonoBehaviour
{
    private int totalPlayers = 1;
    [SerializeField] private Sprite[] profilePictures;
    GameManager gm;

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

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        gm.StartGame();
    }
}
