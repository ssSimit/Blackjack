using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Dealing,
        PlayerTurn,
        DealerTurn,
        RoundEnd
    }

    public GameState gameState;
    private int currentPlayerIndex;

    [Header("Assets")]
    [SerializeField] private Sprite[] cardSprites;

    private Deck deck;
    public List<Hand> playerHands;
    public Hand dealerHand;

    public int playerCount;

    [SerializeField] TextMeshProUGUI[] playerHandValueTexts;
    [SerializeField] TextMeshProUGUI dealerHandValueText;

    [SerializeField] private CardView cardViewPrefab;
    [SerializeField] private RectTransform[] playerCardAnchors;
    [SerializeField] private RectTransform dealerCardAnchor;

    CardView dealerFirstCardView;

    void Awake()
    {
        Bootstrap();
    }

    void Start()
    {
        SpawnOpeningCardViews();
    }

    private void Bootstrap()
    {
        InitializeCardSprites();
        InitializeDeck();
        InitializeHands();
        DealOpeningCards();
    }

    private void InitializeCardSprites()
    {
        CardSpriteDatabase.Initialize(cardSprites);
    }

    private void InitializeDeck()
    {
        deck = new Deck();
        deck.InitializeDeck();
        deck.Shuffle();
    }

    private void InitializeHands()
    {
        playerHands = new List<Hand>();

        for (int i = 0; i < playerCount; i++)
            playerHands.Add(new Hand());

        dealerHand = new Hand();
    }

    private void DealOpeningCards()
    {
        foreach (Hand hand in playerHands)
        {
            hand.cards.Add(deck.DrawCard());
            hand.cards.Add(deck.DrawCard());
        }

        dealerHand.cards.Add(deck.DrawCard());
        dealerHand.cards.Add(deck.DrawCard());
    }

    private void SpawnOpeningCardViews()
    {
        for (int p = 0; p < playerHands.Count; p++)
        {
            Hand hand = playerHands[p];
            playerHandValueTexts[p].text = $"Total: {hand.GetHandValue()}";

            for (int c = 0; c < hand.cards.Count; c++)
            {
                Card card = hand.cards[c];
                CardView view = Instantiate(cardViewPrefab, playerCardAnchors[p]);
                view.SetCard(card);
            }
        }

        int dealerCardIndex = 0;
        foreach (Card card in dealerHand.cards)
        {
            CardView view = Instantiate(cardViewPrefab, dealerCardAnchor);
            if (dealerCardIndex == 0)
            {
                view.SetBackside();
                dealerFirstCardView = view;
            }
            else
            {
                view.SetCard(card);
            }
            dealerCardIndex++;
        }
        StartPlayerTurns();
    }

    private void StartPlayerTurns()
    {
        currentPlayerIndex = 0;
        gameState = GameState.PlayerTurn;
    }

    public void OnHitPressed()
    {
        if (gameState != GameState.PlayerTurn)
            return;

        Hand hand = playerHands[currentPlayerIndex];

        Card card = deck.DrawCard();
        hand.cards.Add(card);

        SpawnSingleCardView(currentPlayerIndex, card);

        CheckPlayerBustOrContinue(hand);
        playerHandValueTexts[currentPlayerIndex].text = $"Total: {hand.GetHandValue()}";

    }


    private void CheckPlayerBustOrContinue(Hand hand)
    {
        int value = hand.GetHandValue();

        if (value > 21)
        {
            Debug.Log($"Player {currentPlayerIndex + 1} Busts with {value}!");
            // Bust
            EndCurrentPlayerTurn();
        }
    }

    public void OnStandPressed()
    {
        if (gameState != GameState.PlayerTurn)
            return;

        EndCurrentPlayerTurn();
    }

    private void EndCurrentPlayerTurn()
    {
        currentPlayerIndex++;

        if (currentPlayerIndex < playerHands.Count)
        {
            // Next player
            // HighlightActivePlayer(currentPlayerIndex);
        }
        else
        {
            // All players done
            StartDealerTurn();
        }
    }

    private void StartDealerTurn()
    {
        gameState = GameState.DealerTurn;

        //  EnablePlayerInput(false);
        dealerFirstCardView.SetCard(dealerHand.cards[0]);
        StartCoroutine(DealerPlayRoutine());
    }

    private IEnumerator DealerPlayRoutine()
    {
        while (dealerHand.GetHandValue() < 17)
        {
            yield return new WaitForSeconds(0.5f);

            Card card = deck.DrawCard();
            dealerHand.cards.Add(card);
            SpawnDealerCardView(card);
        }

        ResolveRound();
    }
    private void SpawnDealerCardView(Card card)
    {
        CardView view = Instantiate(cardViewPrefab, dealerCardAnchor);
        view.SetCard(card);
        dealerHandValueText.text = $"Total: {dealerHand.GetHandValue()}";
    }


    private void SpawnSingleCardView(int playerIndex, Card card)
    {
        CardView view = Instantiate(cardViewPrefab, playerCardAnchors[playerIndex]);
        view.SetCard(card);
    }

    private void ResolveRound()
    {
        gameState = GameState.RoundEnd;

        int dealerValue = dealerHand.GetHandValue();

        for (int i = 0; i < playerHands.Count; i++)
        {
            Hand playerHand = playerHands[i];
            int playerValue = playerHand.GetHandValue();

            if (playerValue > 21)
            {
                // Player busts → lose
                Debug.Log($"Player {i + 1} loses with bust {playerValue}.");
            }
            else if (dealerValue > 21)
            {
                // Dealer busts → win
                Debug.Log($"Player {i + 1} wins! Dealer busts with {dealerValue}.");
            }
            else if (playerValue > dealerValue)
            {
                // Player wins
                Debug.Log($"Player {i + 1} wins with {playerValue} against dealer's {dealerValue}.");
            }
            else if (playerValue < dealerValue)
            {
                // Player loses
                Debug.Log($"Player {i + 1} loses with {playerValue} against dealer's {dealerValue}.");
            }
            else
            {
                // Push
                Debug.Log($"Player {i + 1} pushes with {playerValue} against dealer's {dealerValue}.");
            }
        }

    }

}