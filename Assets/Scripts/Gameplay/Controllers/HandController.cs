using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandView view;

    [Header("Starting Hand")]
    [SerializeField] private int startingHandSize = 5;
    [SerializeField] private CardConfiguration defaultCardConfig;
    [SerializeField] private List<CardConfiguration> deck;

    [Header("Reorder Settings")]
    [SerializeField] private float maxHeightOffset = 3.5f; // Hauteur d'une carte

    private Hand hand = new Hand();
    private bool isSubscribed = false;

    private void Start()
    {
        // Tirer les cartes de départ
        if (startingHandSize > 0)
        {
            List<Card> tempCards = CreateCardFromDeck(deck, startingHandSize);
            foreach (Card card in tempCards){
                AddCard(card);
            }
        }

        // S'abonner aux événements
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        // Vérifier que l'instance existe ET que l'application ne se ferme pas
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
        // Ne tenter de se désabonner que si on était abonné et que l'instance existe encore
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

    /// <summary>
    /// Réorganise les cartes en fonction de la position de drag
    /// </summary>
    private void OnUpdateCardIndexRequested(GameObject cardGO, Vector3 worldPosition)
    {
        CardData cardData = cardGO.GetComponent<CardData>();
        if (cardData == null || cardData.CardInfo == null) return;

        // Vérifier si la carte est trop haute par rapport à sa position initiale
        float heightDifference = worldPosition.y - cardData.positionInitiale.y;
        
        // Si la carte est trop haute (au-delà de la hauteur d'une carte), on ne réorganise pas
        if (heightDifference > maxHeightOffset)
        {
            return;
        }

        // Calculer le nouvel index basé sur la position mondiale
        int newIndex = CalculateCardIndexFromPosition(worldPosition);
        
        // Réorganiser la carte dans le modèle
        hand.ReorderCard(cardData.CardInfo, newIndex);
        
        // Mettre à jour l'affichage
        view.UpdateDisplay(hand.Cards);
    }

    /// <summary>
    /// Calcule l'index de la carte en fonction de sa position X
    /// </summary>
    private int CalculateCardIndexFromPosition(Vector3 worldPosition)
    {
        // Si la main est vide, retourner 0
        if (hand.Count == 0) return 0;

        // Trouver l'index le plus proche basé sur la position X
        float minDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand.Cards[i];
            GameObject cardGO = view.GetCardGameObject(card);
            
            if (cardGO != null)
            {
                CardData cardData = cardGO.GetComponent<CardData>();
                if (cardData != null)
                {
                    float distance = Mathf.Abs(cardData.positionInitiale.x - worldPosition.x);
                    
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestIndex = i;
                    }
                }
            }
        }

        return closestIndex;
    }

    public void RemoveAllCards()
    {
        hand.Clear();
        view.UpdateDisplay(hand.Cards);
    }

    private Card CreateCardFromConfiguration(CardConfiguration config)
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

    private List<Card> CreateCardFromDeck(List<CardConfiguration> deck, int handSize)
    {
        if (deck == null || deck.Count == 0) return null;

        List<Card> cards = new List<Card>();

        for(int i = 0; i < handSize; i++)
        {
            cards.Add(
                new Card
                {
                    Id = System.Guid.NewGuid().ToString(),
                    Name = deck[i].cardName,
                    CardFrontImage = deck[i].frontSprite,
                    CardBackImage = deck[i].backSprite
                }
            );
        }

        return cards;
    }

    [ContextMenu("Add Test Card")]
    private void AddTestCard()
    {
        Card testCard = CreateCardFromConfiguration(defaultCardConfig);
        AddCard(testCard);
    }

    [ContextMenu("Remove Last Card")]
    private void RemoveLastCard()
    {
        if (hand.Count > 0)
        {
            RemoveCard(hand.Cards[hand.Count - 1]);
        }
    }
}
