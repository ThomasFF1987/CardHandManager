using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

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
    private DrawHandCommand drawHandCommand;

    private void Start()
    {
        // S'abonner aux événements
        SubscribeToEvents();
    }

    private void Update()
    {
        // Écouter la touche G pour piocher/redessiner les cartes
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            DrawInitialHand();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Pioche/Repioche la main initiale en utilisant le pattern Command
    /// </summary>
    private void DrawInitialHand()
    {
        if (startingHandSize > 0)
        {
            // Vider la main existante
            hand.Clear();
            view.UpdateDisplay(hand.Cards);
            
            // Générer de nouvelles cartes
            List<Card> tempCards = CreateCardFromDeck(deck, startingHandSize);
            drawHandCommand = new DrawHandCommand(hand, view, tempCards);
            drawHandCommand.Execute();
        }
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

    private List<Card> CreateCardFromDeck(List<CardConfiguration> cardDeck, int numberOfCard)
    {
        List<Card> cardsGenerated = new List<Card>();
        
        for (int i = 0; i < numberOfCard; i++)
        {
            if (cardDeck != null && cardDeck.Count > 0)
            {
                int randomIndex = Random.Range(0, cardDeck.Count);
                CardConfiguration config = cardDeck[randomIndex];
                cardsGenerated.Add(new Card
                {
                    Id = System.Guid.NewGuid().ToString(),
                    Name = config.cardName,
                    CardFrontImage = config.frontSprite,
                    CardBackImage = config.backSprite
                });
            }
            else if (defaultCardConfig != null)
            {
                cardsGenerated.Add(new Card
                {
                    Id = System.Guid.NewGuid().ToString(),
                    Name = defaultCardConfig.cardName,
                    CardFrontImage = defaultCardConfig.frontSprite,
                    CardBackImage = defaultCardConfig.backSprite
                });
            }
        }
        
        return cardsGenerated;
    }
}
