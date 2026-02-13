using UnityEngine;

/// <summary>
/// État lorsque la carte est au repos dans la main
/// </summary>
public class CardIdleState : ICardState
{
    private readonly CardStateMachine stateMachine;
    
    public string StateName => "Idle";

    public CardIdleState(CardStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        // Réinitialiser l'échelle
        if (stateMachine.CardAnimator != null)
        {
            stateMachine.CardAnimator.ResetScale();
        }
    }

    public void OnUpdate()
    {
        // Rien de spécial en idle
    }

    public void OnExit()
    {
        // Nettoyage si nécessaire
    }
}