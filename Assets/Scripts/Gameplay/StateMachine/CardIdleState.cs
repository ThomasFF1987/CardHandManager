using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// CARDIDLESTATE - État de repos de la carte
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - État par défaut quand la carte est dans la main
/// - Position/rotation définies par le HandView layout
/// - Aucune interaction en cours
/// 
/// 📦 RESPONSABILITÉS :
/// - OnEnter() : Anime vers la position cible
/// - OnUpdate() : État passif
/// - OnExit() : Rien (transition vers Hover/Dragging)
/// 
/// 📊 TRANSITIONS :
/// Start → Idle (état initial)
/// Hover → Idle (souris sort)
/// Dragging → Idle (relâche souris sans jouer)
/// 
/// ═══════════════════════════════════════════════════════════════════════════
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
        // Animer vers la position de repos
        if (stateMachine.CardAnimator != null)
        {
            stateMachine.CardAnimator.AnimateToTargetPosition();
        }
    }

    public void OnUpdate()
    {
        // Attendre interaction
    }

    public void OnExit()
    {
        // Transition vers autre état
    }
}