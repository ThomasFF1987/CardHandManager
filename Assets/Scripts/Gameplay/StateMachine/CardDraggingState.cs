using UnityEngine;

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
        // Suivre la position de la souris
        if (mainCamera != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Mathf.Abs(mainCamera.transform.position.z)
            ));
            
            stateMachine.Transform.position = mousePosition;

            // Notifier le changement de position pour réorganiser la main
            if (CardEventBus.Instance != null)
            {
                CardEventBus.Instance.RaiseUpdateCardIndex(stateMachine.gameObject, mousePosition);
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