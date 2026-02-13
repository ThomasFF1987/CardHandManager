using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// État lorsque la carte est en train d'être déplacée
/// </summary>
public class CardDraggingState : ICardState
{
    private readonly CardStateMachine stateMachine;
    private Camera mainCamera;
    
    public string StateName => "Dragging";

    public CardDraggingState(CardStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        mainCamera = Camera.main;
        
        // Arrêter toute animation en cours
        if (stateMachine.CardAnimator != null)
        {
            stateMachine.CardAnimator.StopAllAnimations();
        }
    }

    public void OnUpdate()
    {
        // Suivre la position de la souris avec le nouveau Input System
        if (mainCamera != null && Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                mousePosition.x,
                mousePosition.y,
                Mathf.Abs(mainCamera.transform.position.z)
            ));
            
            stateMachine.Transform.position = worldPosition;

            // Notifier le changement de position pour réorganiser la main
            if (CardEventBus.Instance != null)
            {
                CardEventBus.Instance.RaiseUpdateCardIndex(stateMachine.gameObject, worldPosition);
            }
        }
    }

    public void OnExit()
    {
        // Restaurer le sorting order
        if (stateMachine.CardData != null)
        {
            stateMachine.CardData.frontSpriteRenderer.sortingOrder = stateMachine.CardData.sortingOrderInitiale;
            stateMachine.CardData.backSpriteRenderer.sortingOrder = stateMachine.CardData.sortingOrderInitiale;
        }
    }
}