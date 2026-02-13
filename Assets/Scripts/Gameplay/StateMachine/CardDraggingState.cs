using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// État lorsque la carte est en train d'être déplacée
/// </summary>
public class CardDraggingState : ICardState
{
    private readonly CardStateMachine stateMachine;
    private Camera mainCamera;
    private Vector3 dragOffset;
    
    public string StateName => "Dragging";

    public CardDraggingState(CardStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        mainCamera = Camera.main;
        
        // Calculer l'offset initial entre la carte et la souris
        if (mainCamera != null && Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                mousePosition.x,
                mousePosition.y,
                Mathf.Abs(mainCamera.transform.position.z)
            ));
            
            dragOffset = stateMachine.Transform.position - worldPosition;
        }
        
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
            
            stateMachine.Transform.position = worldPosition + dragOffset;

            // Notifier le changement de position pour réorganiser la main
            if (CardEventBus.Instance != null)
            {
                CardEventBus.Instance.RaiseUpdateCardIndex(stateMachine.gameObject, worldPosition + dragOffset);
            }
        }
    }

    public void OnExit()
    {
        // Le sorting order n'est plus modifié, donc pas besoin de le restaurer
    }
}