using UnityEngine;

/// <summary>
/// État lorsque la souris survole la carte
/// </summary>
public class CardHoverState : ICardState
{
    private readonly CardStateMachine stateMachine;
    
    public string StateName => "Hover";

    public CardHoverState(CardStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        // Animer le hover
        if (stateMachine.CardAnimator != null)
        {
            stateMachine.CardAnimator.AnimateHover();
        }
    }

    public void OnUpdate()
    {
        // Continuer l'animation de hover
    }

    public void OnExit()
    {
        // L'animation de sortie sera gérée par le prochain état
    }
}