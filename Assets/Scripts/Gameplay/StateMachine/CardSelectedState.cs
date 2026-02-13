using UnityEngine;

/// <summary>
/// État lorsque la carte vient d'être cliquée (avant le drag)
/// </summary>
public class CardSelectedState : ICardState
{
    private readonly CardStateMachine stateMachine;
    
    public string StateName => "Selected";

    public CardSelectedState(CardStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        // Agrandir la carte et la mettre droite
        if (stateMachine.CardAnimator != null)
        {
            stateMachine.CardAnimator.AnimateSelected();
        }
        
        // Augmenter le sorting order pour être au-dessus
        if (stateMachine.CardData != null)
        {
            stateMachine.CardData.frontSpriteRenderer.sortingOrder = 100;
            stateMachine.CardData.backSpriteRenderer.sortingOrder = 100;
        }
    }

    public void OnUpdate()
    {
        // Attendre qu'on passe en Dragging
    }

    public void OnExit()
    {
        // Rien de spécial
    }
}