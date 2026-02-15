using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardInteraction : MonoBehaviour
{
    public event Action<CardData> OnCardClicked;
    public event Action<CardData> OnCardHovered;
    public event Action<CardData> OnCardUnhovered;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask cardLayerMask = -1;
    
    private CardData cardData;
    private CardStateMachine stateMachine;
    private bool isHovered;
    private bool isDragging;
    
    // Array pour les colliders au lieu de RaycastHit2D
    private readonly Collider2D[] colliderHits = new Collider2D[10];
    private Vector2 cachedMousePosition;
    private Vector3 cachedScreenToWorld;

    // Throttling pour réduire les raycasts
    private const float RAYCAST_INTERVAL = 0.033f;
    private float nextRaycastTime;
    private bool wasMouseOverCardLastFrame;

    private static readonly Dictionary<GameObject, CardData> cardDataCache = new Dictionary<GameObject, CardData>(20);

    private void Awake()
    {
        cardData = GetComponent<CardData>();
        stateMachine = GetComponent<CardStateMachine>();
        
        // Ajouter au cache global
        if (cardData != null)
        {
            cardDataCache[gameObject] = cardData;
        }
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        nextRaycastTime = 0f;
        wasMouseOverCardLastFrame = false;
    }

    private void OnDestroy()
    {
        // Nettoyer le cache
        cardDataCache.Remove(gameObject);
    }

    private void Update()
    {
        if (Mouse.current == null || mainCamera == null) return;
        
        HandleMouseInteraction();
    }

    private void HandleMouseInteraction()
    {
        cachedMousePosition = Mouse.current.position.ReadValue();
        
        // Throttle les raycasts sauf pendant le drag
        bool shouldRaycast = isDragging || Time.time >= nextRaycastTime;
        bool isMouseOverCard = wasMouseOverCardLastFrame;
        
        if (shouldRaycast)
        {
            isMouseOverCard = IsMouseOverCard(cachedMousePosition);
            wasMouseOverCardLastFrame = isMouseOverCard;
            
            if (!isDragging)
            {
                nextRaycastTime = Time.time + RAYCAST_INTERVAL;
            }
        }

        HandleHover(isMouseOverCard);

        if (isMouseOverCard && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleMouseDown();
        }

        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            HandleMouseUp(cachedMousePosition);
        }
    }

    /// <summary>
    /// Vérifie si la souris est au-dessus de cette carte UNIQUEMENT (prend le hit le plus proche)
    /// </summary>
    private bool IsMouseOverCard(Vector2 screenPosition)
    {
        // Convertir la position écran en position monde à la profondeur de la carte
        Vector3 worldPosition;
        
        if (mainCamera.orthographic)
        {
            cachedScreenToWorld.x = screenPosition.x;
            cachedScreenToWorld.y = screenPosition.y;
            cachedScreenToWorld.z = Mathf.Abs(mainCamera.transform.position.z);
            worldPosition = mainCamera.ScreenToWorldPoint(cachedScreenToWorld);
        }
        else
        {
            // En Perspective : calculer la distance réelle jusqu'au plan Z=0 (où sont les cartes)
            float distanceToCardPlane = mainCamera.transform.position.z - transform.position.z;
            cachedScreenToWorld.x = screenPosition.x;
            cachedScreenToWorld.y = screenPosition.y;
            cachedScreenToWorld.z = Mathf.Abs(distanceToCardPlane);
            worldPosition = mainCamera.ScreenToWorldPoint(cachedScreenToWorld);
        }
        
        // Créer un ContactFilter2D avec le layerMask
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(cardLayerMask);
        contactFilter.useLayerMask = true;
        
        // Utiliser OverlapPoint pour détecter toutes les cartes à cette position
        int hitCount = Physics2D.OverlapPoint(
            worldPosition,
            contactFilter,
            colliderHits
        );
        
        if (hitCount == 0) return false;
        
        // Trouver la carte avec le sorting order le plus élevé (celle au-dessus)
        GameObject topCard = null;
        int highestSortingOrder = int.MinValue;
        
        for (int i = 0; i < hitCount; i++)
        {
            if (colliderHits[i] == null) continue;
            
            GameObject hitObject = colliderHits[i].gameObject;
            
            if (cardDataCache.TryGetValue(hitObject, out CardData hitCardData))
            {
                int sortingOrder = hitCardData.frontSpriteRenderer.sortingOrder;
                
                if (sortingOrder > highestSortingOrder)
                {
                    highestSortingOrder = sortingOrder;
                    topCard = hitObject;
                }
            }
        }
        
        // Retourner true seulement si cette carte est la plus haute
        return topCard != null && topCard == gameObject;
    }

    private void HandleHover(bool isMouseOverCard)
    {
        // Mouse Enter
        if (isMouseOverCard && !isHovered && !isDragging)
        {
            isHovered = true;
            if (stateMachine != null && stateMachine.IsInState<CardIdleState>())
            {
                stateMachine.ChangeState(stateMachine.HoverState);
                OnCardHovered?.Invoke(cardData);
            }
        }
        // Mouse Exit
        else if (!isMouseOverCard && isHovered && !isDragging)
        {
            isHovered = false;
            if (stateMachine != null && stateMachine.IsInState<CardHoverState>())
            {
                stateMachine.ChangeState(stateMachine.IdleState);
                
                if (stateMachine.CardAnimator != null)
                {
                    stateMachine.CardAnimator.AnimateUnhover();
                }
                
                OnCardUnhovered?.Invoke(cardData);
            }
        }
    }

    private void HandleMouseDown()
    {
        if (stateMachine != null && (stateMachine.IsInState<CardIdleState>() || stateMachine.IsInState<CardHoverState>()))
        {
            isDragging = true;
            isHovered = false;
            
            stateMachine.ChangeState(stateMachine.SelectedState);
            stateMachine.ChangeState(stateMachine.DraggingState);
            
            OnCardClicked?.Invoke(cardData);
        }
    }

    private void HandleMouseUp(Vector2 mousePosition)
    {
        if (stateMachine != null && stateMachine.IsInState<CardDraggingState>())
        {
            isDragging = false;
            
            GameObject elementBehind = GetUIElementBehind(mousePosition);
            
            if (elementBehind != null && elementBehind.CompareTag("UI_PlayZone"))
            {
                if (CardEventBus.Instance != null)
                {
                    CardEventBus.Instance.RaiseRemoveCard(gameObject);
                }
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
                
                if (stateMachine.CardAnimator != null)
                {
                    stateMachine.CardAnimator.AnimateDeselected();
                }
            }
            
            if (CardEventBus.Instance != null)
            {
                CardEventBus.Instance.RaiseHandLayoutUpdate();
            }
        }
    }

    private GameObject GetUIElementBehind(Vector3 screenPosition)
    {
        return null;
    }
}