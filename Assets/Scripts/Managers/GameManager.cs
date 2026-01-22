using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameState
    {
        Dealing,
        PlayerTurn,
        DealerTurn,
        RoundEnd
    }

    public GameState gameState;
    public int currentPlayerIndex;

    [Header("Assets")]
    [SerializeField] private Sprite[] cardSprites;

    private Deck deck;
    public List<Hand> playerHands;
    public Hand dealerHand;

    public int playerCount;

    public List<TextMeshProUGUI> playerHandValueTexts;
    [SerializeField] TextMeshProUGUI dealerHandValueText;

    [SerializeField] private CardView cardViewPrefab;
    public List<RectTransform> playerCardAnchors;
    public List<HorizontalLayoutGroup> playerHandHorizontalGroups;
    public List<int> playersWithBlackjack;
    [SerializeField] private RectTransform dealerCardAnchor;

    CardView dealerFirstCardView;

    bool allPlayersBust;
    int playersBustedCount;

    public UnityEvent nextPlayerTurnEvent = new UnityEvent();
    public UnityEvent dealersTurn = new UnityEvent();

    public UnityEvent<string, int> sendPlayerFeedback = new UnityEvent<string, int>();
    public UnityEvent<bool, int, bool> sendPlayerWinLoss = new UnityEvent<bool, int, bool>();

    public UnityEvent roundResolved = new UnityEvent();

    void Awake()
    {
        Instance = this;
        Bootstrap();
    }

    // void Start()
    // {
    //     SpawnOpeningCardViews();
    // }

    private void Bootstrap()
    {
        InitializeCardSprites();
        InitializeDeck();
        //  InitializeHands();
        //  DealOpeningCards();
    }

    public void StartGame()
    {
        InitializeHands();
        DealOpeningCards();
        SpawnOpeningCardViews();
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

    public void InitializeHands()
    {
        playerHands = new List<Hand>();

        for (int i = 0; i < playerCount; i++)
            playerHands.Add(new Hand());

        dealerHand = new Hand();
    }

    public void DealOpeningCards()
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
        CheckBlackJack();
    }

    public void OnHitPressed()
    {
        if (gameState != GameState.PlayerTurn)
            return;

        Hand hand = playerHands[currentPlayerIndex];

        Card card = deck.DrawCard();
        hand.cards.Add(card);

        SpawnSingleCardView(currentPlayerIndex, card);
        playerHandHorizontalGroups[currentPlayerIndex].spacing = -(460 + (hand.cards.Count - 2) * 100);

        playerHandValueTexts[currentPlayerIndex].text = $"Total: {hand.GetHandValue()}";

        CheckPlayerBustOrContinue(hand);

    }


    private void CheckPlayerBustOrContinue(Hand hand)
    {
        int value = hand.GetHandValue();

        if (value > 21)
        {
            //  Debug.Log($"Player {currentPlayerIndex + 1} Busts with {value}!");
            playersBustedCount++;
            sendPlayerFeedback.Invoke("Busted!", currentPlayerIndex);
            if (playersBustedCount >= playerHands.Count)
            {
                allPlayersBust = true;
            }
            // Bust
            EndCurrentPlayerTurn();
        }
        else if (value == 21)
        {
            // Debug.Log($"Player {currentPlayerIndex + 1} hits 21!");
            sendPlayerFeedback.Invoke("21!", currentPlayerIndex);
            // Auto-stand on 21
            EndCurrentPlayerTurn();
        }
    }

    void CheckBlackJack()
    {
        //Debug.Log("Checking for Blackjack...");
        Hand hand = playerHands[currentPlayerIndex];
        int value = hand.GetHandValue();
        if (value == 21)
        {
            // Debug.Log($"Player {currentPlayerIndex + 1} has Blackjack!");
            sendPlayerFeedback.Invoke("Blackjack!", currentPlayerIndex);
            playersWithBlackjack.Add(currentPlayerIndex);
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
            nextPlayerTurnEvent.Invoke();
            CheckBlackJack();
        }
        else
        {
            // All players done
            dealersTurn.Invoke();
            StartDealerTurn();
        }
    }

    private void StartDealerTurn()
    {
        gameState = GameState.DealerTurn;

        //  EnablePlayerInput(false);
        dealerFirstCardView.SetCard(dealerHand.cards[0]);
        if (allPlayersBust)
        {
            ResolveRound();
            Debug.Log("All players busted. Dealer wins by default.");
            return;
        }
        StartCoroutine(DealerPlayRoutine());
    }

    private IEnumerator DealerPlayRoutine()
    {
        while (dealerHand.GetHandValue() < 17)
        {
            yield return new WaitForSeconds(1.5f);

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
        // Debug.Log($"Total is now {dealerHand.GetHandValue()}.");
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
            bool playerWon = false;
            bool push = false;

            if (playerValue > 21)
            {
                // Player busts → lose
                //   Debug.Log($"Player {i + 1} loses with bust {playerValue}.");
                sendPlayerFeedback.Invoke("Lost!", i);
                playerWon = false;
            }
            else if (dealerValue > 21)
            {
                // Dealer busts → win
                // Debug.Log($"Player {i + 1} wins! Dealer busts with {dealerValue}.");
                sendPlayerFeedback.Invoke("Won!", i);
                playerWon = true;
            }
            else if (playerValue > dealerValue)
            {
                // Player wins
                // Debug.Log($"Player {i + 1} wins with {playerValue} against dealer's {dealerValue}.");
                sendPlayerFeedback.Invoke("Won!", i);
                playerWon = true;
            }
            else if (playerValue < dealerValue)
            {
                // Player loses
                //   Debug.Log($"Player {i + 1} loses with {playerValue} against dealer's {dealerValue}.");
                sendPlayerFeedback.Invoke("Lost!", i);
                playerWon = false;
            }
            else
            {
                // Push
                //  Debug.Log($"Player {i + 1} pushes with {playerValue} against dealer's {dealerValue}.");
                sendPlayerFeedback.Invoke("Push!", i);
                push = true;
            }
            sendPlayerWinLoss.Invoke(playerWon, i, push);
        }
        roundResolved.Invoke();

    }

}