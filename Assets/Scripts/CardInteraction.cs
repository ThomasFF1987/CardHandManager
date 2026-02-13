using System;
using UnityEngine;

public class CardInteraction : MonoBehaviour
{
    public event Action<CardData> OnCardClicked;
    public event Action<CardData> OnCardHovered;
    public event Action<CardData> OnCardUnhovered;

    [SerializeField] private Camera mainCamera;
    
    private CardData cardData;
    private CardStateMachine stateMachine;

    private void Awake()
    {
        cardData = GetComponent<CardData>();
        stateMachine = GetComponent<CardStateMachine>();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnMouseDown()
    {
        // Passer en état Selected puis Dragging
        if (stateMachine != null && (stateMachine.IsInState<CardIdleState>() || stateMachine.IsInState<CardHoverState>()))
        {
            stateMachine.ChangeState(stateMachine.SelectedState);
            stateMachine.ChangeState(stateMachine.DraggingState);
            
            OnCardClicked?.Invoke(cardData);
        }
    }

    private void OnMouseUp()
    {
        if (stateMachine != null && stateMachine.IsInState<CardDraggingState>())
        {
            // Vérifier si la carte est sur une zone de jeu
            GameObject elementBehind = GetUIElementBehind(Input.mousePosition);
            
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

    private void OnMouseEnter()
    {
        if (stateMachine != null && stateMachine.IsInState<CardIdleState>())
        {
            stateMachine.ChangeState(stateMachine.HoverState);
            OnCardHovered?.Invoke(cardData);
        }
    }

    private void OnMouseExit()
    {
        if (stateMachine != null && stateMachine.IsInState<CardHoverState>())
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            
            // Animer le retour
            if (stateMachine.CardAnimator != null)
            {
                stateMachine.CardAnimator.AnimateUnhover();
            }
            
            OnCardUnhovered?.Invoke(cardData);
        }
    }

    private GameObject GetUIElementBehind(Vector3 screenPosition)
    {
        // Utiliser un raycast UI pour détecter les zones de jeu
        return null; // À compléter selon votre setup UI
    }
}