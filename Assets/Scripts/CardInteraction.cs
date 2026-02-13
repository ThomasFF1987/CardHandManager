using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardInteraction : MonoBehaviour
{
    public event Action<CardData> OnCardClicked;
    public event Action<CardData> OnCardHovered;
    public event Action<CardData> OnCardUnhovered;

    [SerializeField] private Camera mainCamera;
    
    private CardData cardData;
    private CardStateMachine stateMachine;
    private bool isHovered;
    private bool isDragging;

    private void Awake()
    {
        cardData = GetComponent<CardData>();
        stateMachine = GetComponent<CardStateMachine>();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;
        
        HandleMouseInteraction();
    }

    /// <summary>
    /// Gère les interactions souris avec le nouveau Input System
    /// </summary>
    private void HandleMouseInteraction()
    {
        if (mainCamera == null) return;

        // Récupérer la position de la souris avec le nouveau Input System
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        
        // Raycast pour détecter si la souris est sur cette carte
        bool isMouseOverCard = IsMouseOverCard(mousePosition);

        // Gestion du hover
        HandleHover(isMouseOverCard);

        // Gestion du clic
        if (isMouseOverCard && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleMouseDown();
        }

        // Gestion du relâchement
        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            HandleMouseUp(mousePosition);
        }
    }

    /// <summary>
    /// Vérifie si la souris est au-dessus de cette carte
    /// </summary>
    private bool IsMouseOverCard(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        
        return hit.collider != null && hit.collider.gameObject == gameObject;
    }

    /// <summary>
    /// Gère l'entrée et la sortie du hover
    /// </summary>
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

    /// <summary>
    /// Gère le clic sur la carte
    /// </summary>
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

    /// <summary>
    /// Gère le relâchement de la carte
    /// </summary>
    private void HandleMouseUp(Vector2 mousePosition)
    {
        if (stateMachine != null && stateMachine.IsInState<CardDraggingState>())
        {
            isDragging = false;
            
            // Vérifier si la carte est sur une zone de jeu
            GameObject elementBehind = GetUIElementBehind(mousePosition);
            
            if (elementBehind != null && elementBehind.CompareTag("UI_PlayZone"))
            {
                // Jouer la carte (elle sera détruite)
                if (CardEventBus.Instance != null)
                {
                    CardEventBus.Instance.RaiseRemoveCard(gameObject);
                }
            }
            else
            {
                // Retourner à l'état Idle
                stateMachine.ChangeState(stateMachine.IdleState);
                
                // Animer le retour à la position
                if (stateMachine.CardAnimator != null)
                {
                    stateMachine.CardAnimator.AnimateDeselected();
                }
            }
            
            // Notifier la mise à jour du layout
            if (CardEventBus.Instance != null)
            {
                CardEventBus.Instance.RaiseHandLayoutUpdate();
            }
        }
    }

    private GameObject GetUIElementBehind(Vector3 screenPosition)
    {
        // Utiliser un raycast UI pour détecter les zones de jeu
        return null; // À compléter selon votre setup UI
    }
}