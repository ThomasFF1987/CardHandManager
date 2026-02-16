using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// DECKMANAGER - Gestionnaire du deck de cartes (utilise DeckConfiguration)
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Gère le deck de cartes depuis un ScriptableObject DeckConfiguration
/// - Initialise le deck au démarrage
/// - Fournit les cartes pour la pioche
/// - Gère le shuffle du deck
/// 
/// 📦 RESPONSABILITÉS :
/// - LoadDeck() : Charge un deck depuis DeckConfiguration
/// - DrawCards() : Pioche X cartes du deck
/// - Shuffle() : Mélange le deck
/// - RemainingCards : Nombre de cartes restantes
/// 
/// 🔗 UTILISÉ PAR :
/// - HandController : Pour piocher des cartes
/// - DrawHandCommand : Pour obtenir les cartes à ajouter à la main
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Changer de deck en runtime (SwapDeck)
/// - Ajouter un système de discard pile (défausse)
/// - Créer un auto-reshuffle quand le deck est vide
/// - Ajouter des événements OnDeckEmpty, OnCardDrawn
/// - Implémenter différents types de decks (constructed, draft)
/// 
/// ⚙️ CONFIGURATION UNITY :
/// - Assignez un DeckConfiguration dans l'Inspector
/// - Le deck sera chargé automatiquement au Start
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class DeckManager : MonoBehaviour
{
    [Header("Deck Configuration")]
    [Tooltip("Configuration du deck à charger (ScriptableObject)")]
    [SerializeField] private DeckConfiguration deckConfiguration;
    
    [Tooltip("Configuration par défaut si le deck principal est vide")]
    [SerializeField] private CardConfiguration defaultCardConfig;
    
    [Header("Runtime Options")]
    [Tooltip("Mode debug : génère des cartes aléatoires au lieu de piocher du deck")]
    [SerializeField] private bool useRandomDraw = false;

    private Deck deck = new Deck();
    private DeckConfiguration currentDeckConfig;
    
    public int RemainingCards => deck.Count;
    public bool IsEmpty => deck.Count == 0;
    public string CurrentDeckName => currentDeckConfig != null ? currentDeckConfig.deckName : "No Deck";

    private void Awake()
    {
        LoadDeck(deckConfiguration);
    }

    /// <summary>
    /// Charge un deck depuis une DeckConfiguration
    /// </summary>
    public void LoadDeck(DeckConfiguration config)
    {
        if (config == null)
        {
            Debug.LogError("DeckConfiguration est null ! Impossible de charger le deck.");
            return;
        }

        if (!config.IsValid())
        {
            Debug.LogError($"DeckConfiguration '{config.deckName}' n'est pas valide !");
            return;
        }

        currentDeckConfig = config;
        deck = new Deck();

        // Copier les cartes depuis la configuration
        List<CardConfiguration> cardConfigs = config.GetCardsCopy();
        
        foreach (CardConfiguration cardConfig in cardConfigs)
        {
            Card card = CreateCardFromConfig(cardConfig);
            deck.AddCard(card);
        }

        // Mélanger si nécessaire
        if (config.shuffleOnLoad)
        {
            Shuffle();
        }

        Debug.Log($"✅ Deck '{config.deckName}' chargé : {deck.Count} cartes");
    }

    /// <summary>
    /// Change de deck en runtime
    /// </summary>
    public void SwapDeck(DeckConfiguration newConfig)
    {
        if (newConfig == null) return;
        
        Debug.Log($"🔄 Changement de deck : {CurrentDeckName} → {newConfig.deckName}");
        LoadDeck(newConfig);
    }

    /// <summary>
    /// Pioche une carte du deck
    /// </summary>
    public Card DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("Tentative de piocher depuis un deck vide !");
            return null;
        }
        
        return deck.DrawCard();
    }

    /// <summary>
    /// Pioche plusieurs cartes du deck
    /// </summary>
    public List<Card> DrawCards(int count)
    {
        List<Card> drawnCards = new List<Card>();
        
        int cardsToDraw = Mathf.Min(count, deck.Count);
        
        for (int i = 0; i < cardsToDraw; i++)
        {
            Card card = DrawCard();
            if (card != null)
            {
                drawnCards.Add(card);
            }
        }
        
        if (cardsToDraw < count)
        {
            Debug.LogWarning($"⚠️ Seulement {cardsToDraw}/{count} cartes piochées (deck épuisé)");
        }
        
        return drawnCards;
    }

    /// <summary>
    /// Génère des cartes aléatoires depuis la config (pour debug/testing)
    /// </summary>
    public List<Card> DrawRandomCards(int count)
    {
        if (currentDeckConfig == null || currentDeckConfig.cards == null || currentDeckConfig.cards.Count == 0)
        {
            Debug.LogWarning("Pas de configuration de deck pour générer des cartes aléatoires");
            return new List<Card>();
        }

        List<Card> randomCards = new List<Card>();
        
        for (int i = 0; i < count; i++)
        {
            CardConfiguration config = GetRandomCardConfig();
            if (config != null)
            {
                randomCards.Add(CreateCardFromConfig(config));
            }
        }
        
        return randomCards;
    }

    /// <summary>
    /// Mélange le deck
    /// </summary>
    public void Shuffle()
    {
        deck.Shuffle();
        Debug.Log($"🔀 Deck '{CurrentDeckName}' mélangé. Cartes restantes : {deck.Count}");
    }

    /// <summary>
    /// Remet une carte dans le deck (pour Undo ou mécaniques spéciales)
    /// </summary>
    public void AddCardToDeck(Card card)
    {
        if (card != null)
        {
            deck.AddCard(card);
        }
    }

    /// <summary>
    /// Réinitialise le deck avec la configuration actuelle
    /// </summary>
    public void ResetDeck()
    {
        if (currentDeckConfig != null)
        {
            LoadDeck(currentDeckConfig);
        }
    }

    /// <summary>
    /// Récupère une configuration aléatoire depuis le deck actuel
    /// </summary>
    private CardConfiguration GetRandomCardConfig()
    {
        if (currentDeckConfig != null && currentDeckConfig.cards.Count > 0)
        {
            return currentDeckConfig.cards[Random.Range(0, currentDeckConfig.cards.Count)];
        }
        return defaultCardConfig;
    }

    /// <summary>
    /// Crée une carte depuis une configuration
    /// </summary>
    private Card CreateCardFromConfig(CardConfiguration config)
    {
        if (config == null) return null;
        
        return new Card
        {
            Id = System.Guid.NewGuid().ToString(),
            Name = config.cardName,
            CardFrontImage = config.frontSprite,
            CardBackImage = config.backSprite
        };
    }

#if UNITY_EDITOR
    [ContextMenu("Log Deck Status")]
    private void LogDeckStatus()
    {
        Debug.Log($"=== DeckManager Status ===");
        Debug.Log($"Current Deck: {CurrentDeckName}");
        Debug.Log($"Remaining Cards: {RemainingCards}");
        Debug.Log($"Is Empty: {IsEmpty}");
        Debug.Log($"Random Draw Mode: {useRandomDraw}");
    }

    [ContextMenu("Reset Deck")]
    private void ResetDeckContextMenu()
    {
        ResetDeck();
    }
#endif
}