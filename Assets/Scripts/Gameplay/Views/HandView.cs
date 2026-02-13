using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    [Header("Layout Parameters")]
    [SerializeField] [Range(1f, 50f)] private float spacing = 5f;
    [SerializeField] [Range(0f, 100f)] private float angleMax = 45f;
    
    [Header("References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform handTransform;
    
    private Dictionary<Card, GameObject> cardGameObjects = new Dictionary<Card, GameObject>();
    
    private class CardComponents
    {
        public CardData Data;
        public CardAnimator Animator;
        public CardStateMachine StateMachine;
        
        public CardComponents(GameObject cardGO)
        {
            Data = cardGO.GetComponent<CardData>();
            Animator = cardGO.GetComponent<CardAnimator>();
            StateMachine = cardGO.GetComponent<CardStateMachine>();
        }
    }
    
    private Dictionary<Card, CardComponents> cardComponents = new Dictionary<Card, CardComponents>();
    private HashSet<Card> currentCardsSet = new HashSet<Card>();
    
    public void UpdateDisplay(IReadOnlyList<Card> cards)
    {
        RemoveObsoleteCards(cards);
        AddNewCards(cards);
        UpdateLayout(cards);
    }
    
    /// <summary>
    /// Supprime les cartes qui ne sont plus dans la main
    /// </summary>
    private void RemoveObsoleteCards(IReadOnlyList<Card> cards)
    {
        // Créer un HashSet pour des recherches O(1)
        currentCardsSet.Clear();
        for (int i = 0; i < cards.Count; i++)
        {
            currentCardsSet.Add(cards[i]);
        }
        
        // Identifier et supprimer les cartes obsolètes
        List<Card> cardsToRemove = new List<Card>();
        foreach (var kvp in cardGameObjects)
        {
            if (!currentCardsSet.Contains(kvp.Key))
            {
                Destroy(kvp.Value);
                cardsToRemove.Add(kvp.Key);
            }
        }
        
        // Nettoyer les dictionnaires
        for (int i = 0; i < cardsToRemove.Count; i++)
        {
            cardGameObjects.Remove(cardsToRemove[i]);
            cardComponents.Remove(cardsToRemove[i]);
        }
    }
    
    /// <summary>
    /// Ajoute les nouvelles cartes à la main
    /// </summary>
    private void AddNewCards(IReadOnlyList<Card> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (!cardGameObjects.ContainsKey(card))
            {
                GameObject cardGO = Instantiate(cardPrefab, handTransform);
                cardGameObjects[card] = cardGO;
                
                CardComponents components = new CardComponents(cardGO);
                cardComponents[card] = components;
                
                if (components.Data != null)
                {
                    components.Data.CardInfo = card;
                    if (card.CardImage != null)
                    {
                        components.Data.frontSpriteRenderer.sprite = card.CardImage;
                    }
                    components.Data.ShowFront();
                }
            }
        }
    }
    
    private void UpdateLayout(IReadOnlyList<Card> cards)
    {
        int count = cards.Count;
        if (count == 0) return;

        float angleStep = angleMax / Mathf.Max(1, count - 1);
        float startAngle = -angleMax / 2;

        for (int i = 0; i < count; i++)
        {
            Card card = cards[i];
            if (!cardComponents.ContainsKey(card)) continue;
            
            CardComponents components = cardComponents[card];
            
            CalculateCardPosition(i, count, angleStep, startAngle, out Vector3 targetPosition, out Quaternion targetRotation);
            UpdateCardVisuals(components, i, targetPosition, targetRotation);
            AnimateCard(components, targetPosition, targetRotation);
        }
    }
    
    /// <summary>
    /// Calcule la position et rotation d'une carte dans l'éventail
    /// </summary>
    private void CalculateCardPosition(int index, int totalCards, float angleStep, float startAngle, 
                                      out Vector3 position, out Quaternion rotation)
    {
        float angle = startAngle + index * angleStep;
        float xOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * spacing;
        float yOffset = Mathf.Cos(-angle * Mathf.Deg2Rad) * spacing;

        if (totalCards == 1)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
        }
        else if (totalCards == 2)
        {
            position = new Vector3(xOffset / 2, yOffset, 0);
            rotation = Quaternion.Euler(0, 0, -angle);
            position -= new Vector3(0, spacing, index * 0.01f);
        }
        else
        {
            position = new Vector3(xOffset, yOffset, 0);
            rotation = Quaternion.Euler(0, 0, -angle);
            position -= new Vector3(0, spacing, index * 0.01f);
        }
    }
    
    /// <summary>
    /// Met à jour les propriétés visuelles de la carte (sorting order, positions)
    /// </summary>
    private void UpdateCardVisuals(CardComponents components, int sortingOrder, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (components.Data != null)
        {
            components.Data.frontSpriteRenderer.sortingOrder = sortingOrder;
            components.Data.backSpriteRenderer.sortingOrder = sortingOrder;
            components.Data.positionInitiale = targetPosition;
            components.Data.rotationInitiale = targetRotation;
            components.Data.sortingOrderInitiale = sortingOrder;
        }
    }
    
    /// <summary>
    /// Anime la carte vers sa position cible si elle n'est pas en interaction
    /// </summary>
    private void AnimateCard(CardComponents components, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (components.StateMachine == null || components.Animator == null) return;

        bool isBeingDragged = components.StateMachine.IsInState<CardDraggingState>() ||
                             components.StateMachine.IsInState<CardSelectedState>();
        
        if (!isBeingDragged)
        {
            components.Animator.SetTargetTransform(targetPosition, targetRotation);
            components.Animator.AnimateToTargetPosition();
        }
        else
        {
            // Juste mettre à jour la cible pour quand elle sera relâchée
            components.Animator.SetTargetTransform(targetPosition, targetRotation);
        }
    }
}
