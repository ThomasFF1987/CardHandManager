using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandView view;

    [Header("Starting Hand")]
    [SerializeField] private int startingHandSize = 5;
    [SerializeField] private CardConfiguration defaultCardConfig;
    [SerializeField] private List<CardConfiguration> decks;

    private Hand hand = new Hand();
    private bool isSubscribed = false;

    private void Start()
    {
        // Tirer les cartes de départ
        if (startingHandSize > 0)
        {
            List<Card> tempCards = CreateCardFromDeck(decks, startingHandSize);
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
            CardImage = config.frontSprite
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
                    CardImage = deck[i].frontSprite
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
