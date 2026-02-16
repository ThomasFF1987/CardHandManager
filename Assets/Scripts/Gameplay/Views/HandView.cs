using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// HANDVIEW - Vue de la main en éventail (Fan Layout)
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Affichage visuel de la main du joueur
/// - Calcule et applique le layout en arc de cercle
/// - Instancie/détruit les GameObjects des cartes
/// - Couche "View" dans le pattern MVC
/// 
/// 📦 RESPONSABILITÉS :
/// - UpdateDisplay() : Point d'entrée pour rafraîchir l'affichage
/// - UpdateLayout() : Calcule positions/rotations en éventail
/// - AddNewCards() : Instancie les nouveaux GameObjects de cartes
/// - RemoveObsoleteCards() : Détruit les cartes supprimées
/// - GetCardGameObject() : Récupère le GameObject d'une carte
/// 
/// 🔗 UTILISÉ PAR :
/// - HandController : Appelle UpdateDisplay() après chaque modification
/// - DrawHandCommand : Met à jour l'affichage après pioche
/// 
/// 📐 ALGORITHME LAYOUT :
/// 1. Calcule l'angle total de l'éventail basé sur le nombre de cartes
/// 2. Pour chaque carte, calcule :
///    - Position en arc de cercle (sin/cos)
///    - Rotation pour suivre la courbe
///    - Sorting order (cartes centrales au-dessus)
/// 3. Applique via CardAnimator.SetTargetTransform()
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Ajuster spacing et angleMax en runtime via les propriétés
/// - Créer d'autres layouts (ligne droite, grille, poker hand)
/// - Ajouter des animations de transition (cards shuffling)
/// - Implémenter un zoom sur la carte survolée
/// - Créer un layout différent pour mobile (vertical)
/// - Ajouter des effets de particules sur les cartes
/// 
/// ⚙️ CONFIGURATION INSPECTOR :
/// - spacing : Espacement entre les cartes (1-50)
/// - angleMax : Angle maximum de l'éventail (0-100°)
/// - cardPrefab : Prefab de carte à instancier
/// - handTransform : Parent des cartes
/// 
/// 📊 DICTIONNAIRES :
/// - cardGameObjects : Map Card → GameObject
/// - cardComponents : Map Card → (CardData, CardAnimator, CardStateMachine)
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
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
    private IReadOnlyList<Card> cachedCards;

    // Events pour détecter les changements de layout
    public event Action OnSpacingChanged;
    public event Action OnAngleMaxChanged;

    /// <summary>
    /// Propriété pour modifier le spacing en runtime avec notification de changement
    /// </summary>
    public float Spacing
    {
        get => spacing;
        set
        {
            if (!Mathf.Approximately(spacing, value))
            {
                spacing = Mathf.Clamp(value, 1f, 50f);
                OnSpacingChanged?.Invoke();
                RefreshLayout();
            }
        }
    }

    /// <summary>
    /// Propriété pour modifier l'angle max en runtime avec notification de changement
    /// </summary>
    public float AngleMax
    {
        get => angleMax;
        set
        {
            if (!Mathf.Approximately(angleMax, value))
            {
                angleMax = Mathf.Clamp(value, 0f, 100f);
                OnAngleMaxChanged?.Invoke();
                RefreshLayout();
            }
        }
    }

    public void UpdateDisplay(IReadOnlyList<Card> cards)
    {
        cachedCards = cards;
        RemoveObsoleteCards(cards);
        AddNewCards(cards);
        UpdateLayout(cards);
    }

    /// <summary>
    /// Rafraîchit le layout avec les cartes actuelles (utilisé quand spacing ou angleMax change)
    /// </summary>
    private void RefreshLayout()
    {
        if (cachedCards != null && cachedCards.Count > 0)
        {
            UpdateLayout(cachedCards);
        }
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
                    if (card.CardFrontImage != null && card.CardBackImage != null)
                    {
                        components.Data.SetFrontSprite(card.CardFrontImage);
                        components.Data.SetBackSprite(card.CardBackImage);
                    }
                    //components.Data.ShowFront();
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

    /// <summary>
    /// Récupère le GameObject associé à une carte
    /// </summary>
    public GameObject GetCardGameObject(Card card)
    {
        return cardGameObjects.TryGetValue(card, out GameObject cardGO) ? cardGO : null;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Permet de modifier spacing et angleMax en temps réel dans l'Inspector en mode Play
    /// </summary>
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            RefreshLayout();
        }
    }
#endif
}
