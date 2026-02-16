using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contrôleur pour gérer un deck de cartes avec son affichage visuel
/// </summary>
public class DeckController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeckConfiguration deckConfiguration;
    [SerializeField] private Transform deckTransform;
    [SerializeField] private GameObject cardBackPrefab;

    [Header("Visual Settings")]
    [SerializeField] private float spaceBetweenCards = 0.03f;
    [SerializeField] private float varianceRotation = 1f;

    private Deck deck = new Deck();
    private List<GameObject> visualCards = new List<GameObject>();

    private void Start()
    {
        if (deckConfiguration != null)
        {
            InitializeDeck();
        }
    }

    /// <summary>
    /// Initialise le deck à partir de la configuration
    /// </summary>
    public void InitializeDeck()
    {
        deck = new Deck();

        foreach (var cardConfig in deckConfiguration.cards)
        {
            Card card = new Card
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = cardConfig.cardName,
                CardFrontImage = cardConfig.frontSprite,
                CardBackImage = cardConfig.backSprite
            };
            deck.AddCard(card);
        }

        UpdateVisualDeck();
    }

    /// <summary>
    /// Tire X cartes du deck
    /// </summary>
    public List<Card> DrawCards(int count)
    {
        List<Card> drawnCards = new List<Card>();

        for (int i = 0; i < count && deck.Cards.Count > 0; i++)
        {
            Card card = deck.DrawCard();
            if (card != null)
            {
                drawnCards.Add(card);
            }
        }

        UpdateVisualDeck();
        return drawnCards;
    }

    /// <summary>
    /// Mélange le deck
    /// </summary>
    public void Shuffle()
    {
        deck.Shuffle();
        UpdateVisualDeck();
    }

    /// <summary>
    /// Remet des cartes en haut du deck
    /// </summary>
    public void PutCardsOnTop(List<Card> cards)
    {
        foreach (var card in cards)
        {
            deck.AddCard(card); // Ajouter à Deck.cs une méthode AddCardToTop si besoin
        }
        UpdateVisualDeck();
    }

    /// <summary>
    /// Met à jour la représentation visuelle du deck
    /// </summary>
    private void UpdateVisualDeck()
    {
        // Détruire les anciennes visualisations
        foreach (var cardGO in visualCards)
        {
            Destroy(cardGO);
        }
        visualCards.Clear();

        // Créer les nouvelles (limité à 10 pour la performance)
        int visualCount = Mathf.Min(deck.Cards.Count, 10);

        for (int i = 0; i < visualCount; i++)
        {
            GameObject cardBack = Instantiate(cardBackPrefab, deckTransform);
            cardBack.transform.localPosition = new Vector3(0, 0, -spaceBetweenCards * i);
            cardBack.transform.Rotate(new Vector3(0, 0, Random.Range(-varianceRotation, varianceRotation)));
            visualCards.Add(cardBack);
        }
    }

    public int GetDeckCount() => deck.Cards.Count;

    [ContextMenu("Draw 1 Card")]
    private void DebugDrawCard()
    {
        DrawCards(1);
    }

    [ContextMenu("Shuffle Deck")]
    private void DebugShuffle()
    {
        Shuffle();
    }
}
