using UnityEngine;

public class BetButton : MonoBehaviour
{
    public int playerIndex;

    ProfileAndBetManager pbm;
    void Start()
    {
        pbm = ProfileAndBetManager.Instance;
    }

    public void PlaceBet(int amount)
    {
        pbm.playerBets[playerIndex] = amount;
    }

}
