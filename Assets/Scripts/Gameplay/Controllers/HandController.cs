using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// HANDCONTROLLER - Contrôleur de la main du joueur (découplé du deck)
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Orchestre la logique de la main du joueur
/// - Interagit avec DeckManager pour piocher des cartes
/// - Gère les événements de carte via CardEventBus
/// - Couche "Controller" dans le pattern MVC
/// 
/// 📦 RESPONSABILITÉS :
/// - DrawInitialHand() : Pioche la main via DrawHandCommand + DeckManager
/// - AddCard() / RemoveCard() : Ajoute/retire des cartes
/// - OnUpdateCardIndexRequested() : Réorganise les cartes pendant le drag
/// - OnLayoutUpdateRequested() : Rafraîchit l'affichage
/// 
/// 🔗 COMPOSANTS LIÉS :
/// - Hand (modèle) : Gère les données de la main
/// - HandView (vue) : Affiche les cartes en éventail
/// - DeckManager : Fournit les cartes à piocher
/// - CardEventBus : Reçoit les événements (RemoveCard, UpdateCardIndex)
/// - InputHandler : Écoute les inputs clavier
/// - CommandManager : Gère l'historique des commandes
/// 
/// 📊 FLUX D'ÉVÉNEMENTS :
/// Input G → DrawInitialHand() → DrawHandCommand.Execute() → DeckManager.DrawCards() → Hand.AddCard()
/// CardEventBus.RemoveCard → OnRemoveCardRequested() → Hand.RemoveCard()
/// CardDragging → CardEventBus.UpdateCardIndex → OnUpdateCardIndexRequested() → Hand.ReorderCard()
/// 
/// 💡 SÉPARATION DES RESPONSABILITÉS :
/// - ✅ Gestion main → HandController
/// - ✅ Gestion deck → DeckManager
/// - ✅ Gestion input → InputHandler
/// - ✅ Calculs layout → CardLayoutCalculator
/// - ✅ Historique → CommandManager
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class HandController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandView view;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private DeckManager deckManager;

    [Header("Starting Hand")]
    [SerializeField] private int startingHandSize = 5;
    [SerializeField] private bool useRandomDraw = true; // Pour debug/testing

    [Header("Reorder Settings")]
    [SerializeField] private float maxHeightOffset = 3.5f;

    private Hand hand = new Hand();
    private bool isSubscribed = false;
    private CommandManager commandManager = new CommandManager();

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
            inputHandler.OnShuffleHandRequested += ShuffleDeck;
        }
    }

    private void UnsubscribeFromInputs()
    {
        if (inputHandler != null)
        {
            inputHandler.OnDrawHandRequested -= DrawInitialHand;
            inputHandler.OnShuffleHandRequested -= ShuffleDeck;
        }
    }

    /// <summary>
    /// Pioche la main initiale depuis le DeckManager
    /// </summary>
    private void DrawInitialHand()
    {
        if (deckManager == null)
        {
            Debug.LogError("DeckManager non assigné !");
            return;
        }

        if (startingHandSize > 0)
        {
            // Vider la main existante
            hand.Clear();
            view.UpdateDisplay(hand.Cards);
            
            // Piocher depuis le deck via Command
            DrawHandCommand drawCommand = new DrawHandCommand(
                deckManager, 
                hand, 
                view, 
                startingHandSize,
                useRandomDraw
            );
            
            commandManager.ExecuteCommand(drawCommand);
            
            Debug.Log($"Main piochée : {hand.Count} cartes | Deck restant : {deckManager.RemainingCards}");
        }
    }

    /// <summary>
    /// Mélange le deck (touche H)
    /// </summary>
    private void ShuffleDeck()
    {
        if (deckManager != null)
        {
            deckManager.Shuffle();
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

        if (CardLayoutCalculator.IsPositionTooHigh(worldPosition, cardData.positionInitiale, maxHeightOffset))
        {
            return;
        }

        int newIndex = CardLayoutCalculator.CalculateCardIndex(worldPosition, hand, view);
        hand.ReorderCard(cardData.CardInfo, newIndex);
        view.UpdateDisplay(hand.Cards);
    }
}
