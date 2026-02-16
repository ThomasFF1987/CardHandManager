using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// HANDCONTROLLER - Contrôleur simplifié (respecte SRP)
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class HandController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandView view;
    [SerializeField] private InputHandler inputHandler;

    [Header("Starting Hand")]
    [SerializeField] private int startingHandSize = 5;
    [SerializeField] private CardConfiguration defaultCardConfig;
    [SerializeField] private List<CardConfiguration> deck;

    [Header("Reorder Settings")]
    [SerializeField] private float maxHeightOffset = 3.5f;

    private Hand hand = new Hand();
    private bool isSubscribed = false;
    private CommandManager commandManager = new CommandManager(); // ✨ Nouveau

    private void Start()
    {
        SubscribeToEvents();
        SubscribeToInputs();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        UnsubscribeFromInputs();
    }

    private void SubscribeToInputs()
    {
        if (inputHandler != null)
        {
            inputHandler.OnDrawHandRequested += DrawInitialHand;
        }
    }

    private void UnsubscribeFromInputs()
    {
        if (inputHandler != null)
        {
            inputHandler.OnDrawHandRequested -= DrawInitialHand;
        }
    }

    private void DrawInitialHand()
    {
        if (startingHandSize > 0)
        {
            hand.Clear();
            view.UpdateDisplay(hand.Cards);
            
            List<Card> tempCards = CreateCardFromDeck(deck, startingHandSize);
            DrawHandCommand drawHandCommand = new DrawHandCommand(hand, view, tempCards);
            commandManager.ExecuteCommand(drawHandCommand); // ✨ Utilise le CommandManager
        }
    }

    private void SubscribeToEvents()
    {
        if (CardEventBus.Instance != null && !CardEventBus.Instance.Equals(null))
        {
            CardEventBus.Instance.RemoveCard += OnRemoveCardRequested;
            CardEventBus.Instance.HandLayoutToUpdate += OnLayoutUpdateRequested;
            CardEventBus.Instance.UpdateCardIndex += OnUpdateCardIndexRequested;
            isSubscribed = true;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (isSubscribed && CardEventBus.Instance != null && !CardEventBus.Instance.Equals(null))
        {
            CardEventBus.Instance.RemoveCard -= OnRemoveCardRequested;
            CardEventBus.Instance.HandLayoutToUpdate -= OnLayoutUpdateRequested;
            CardEventBus.Instance.UpdateCardIndex -= OnUpdateCardIndexRequested;
            isSubscribed = false;
        }
    }

    public void AddCard(Card card)
    {
        hand.AddCard(card);
        view.UpdateDisplay(hand.Cards);
    }

    public void RemoveCard(Card card)
    {
        hand.RemoveCard(card);
        view.UpdateDisplay(hand.Cards);
    }

    private void OnRemoveCardRequested(GameObject cardGO)
    {
        CardData cardData = cardGO.GetComponent<CardData>();
        if (cardData != null && cardData.CardInfo != null)
        {
            RemoveCard(cardData.CardInfo);
        }
    }

    private void OnLayoutUpdateRequested()
    {
        view.UpdateDisplay(hand.Cards);
    }

    private void OnUpdateCardIndexRequested(GameObject cardGO, Vector3 worldPosition)
    {
        CardData cardData = cardGO.GetComponent<CardData>();
        if (cardData == null || cardData.CardInfo == null) return;

        // Utiliser le calculator statique
        if (CardLayoutCalculator.IsPositionTooHigh(worldPosition, cardData.positionInitiale, maxHeightOffset))
        {
            return;
        }

        int newIndex = CardLayoutCalculator.CalculateCardIndex(worldPosition, hand, view);
        hand.ReorderCard(cardData.CardInfo, newIndex);
        view.UpdateDisplay(hand.Cards);
    }

    private List<Card> CreateCardFromDeck(List<CardConfiguration> cardDeck, int numberOfCard)
    {
        List<Card> cardsGenerated = new List<Card>();
        
        for (int i = 0; i < numberOfCard; i++)
        {
            CardConfiguration config = GetRandomCardConfig(cardDeck);
            if (config != null)
            {
                cardsGenerated.Add(CreateCardFromConfig(config));
            }
        }
        
        return cardsGenerated;
    }
    
    // ✨ Extrait la logique de sélection
    private CardConfiguration GetRandomCardConfig(List<CardConfiguration> cardDeck)
    {
        if (cardDeck != null && cardDeck.Count > 0)
        {
            return cardDeck[Random.Range(0, cardDeck.Count)];
        }
        return defaultCardConfig;
    }
    
    // ✨ Extrait la création de Card
    private Card CreateCardFromConfig(CardConfiguration config)
    {
        return new Card
        {
            Id = System.Guid.NewGuid().ToString(),
            Name = config.cardName,
            CardFrontImage = config.frontSprite,
            CardBackImage = config.backSprite
        };
    }
}
