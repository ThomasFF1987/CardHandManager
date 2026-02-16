using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// CARDSTATEMACHINE - Machine à états finie (FSM) pour une carte
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Gère les états d'une carte (Idle, Hover, Selected, Dragging)
/// - Pattern State Machine pour un comportement clair et modulaire
/// - Centralise les transitions d'états
/// 
/// 📦 RESPONSABILITÉS :
/// - ChangeState() : Change l'état actuel (appelle OnExit/OnEnter)
/// - IsInState<T>() : Vérifie l'état actuel (type-safe)
/// - Update() : Appelle OnUpdate() de l'état courant
/// - Expose les états et composants aux ICardState
/// 
/// 🔗 ÉTATS DISPONIBLES :
/// - IdleState : Carte au repos dans la main
/// - HoverState : Carte survolée (monte + augmente sorting order)
/// - SelectedState : Carte sélectionnée (transition courte)
/// - DraggingState : Carte déplacée (suit souris + tilt rotation)
/// 
/// 📊 TRANSITIONS D'ÉTATS :
/// Idle → Hover (souris entre)
/// Hover → Idle (souris sort avec mouvement)
/// Hover → Selected → Dragging (clic souris)
/// Dragging → Idle (relâche souris)
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Ajouter des états : PlayingState, DiscardingState, ExiledState
/// - Créer un système de transitions avec conditions
/// - Ajouter des animations de transition entre états
/// - Implémenter un historique d'états pour debug
/// - Logger les changements d'états pour analytics
/// - Créer un visualiseur d'état machine dans l'Inspector
/// 
/// 🏗️ PATTERN :
/// State Pattern - Permet à un objet de changer son comportement
/// quand son état interne change. L'objet semblera changer de classe.
/// 
/// ⚙️ PROPRIÉTÉS EXPOSÉES :
/// - Transform, CardData, CardAnimator : Accès aux composants
/// - TiltSettings : Configuration de rotation pendant le drag
/// - IdleState, HoverState, SelectedState, DraggingState : Références états
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class CardStateMachine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private CardTiltSettings tiltSettings;
    
    // États
    private CardIdleState idleState;
    private CardHoverState hoverState;
    private CardSelectedState selectedState;
    private CardDraggingState draggingState;
    
    private ICardState currentState;
    
    // Propriétés publiques pour accéder aux composants
    public Transform Transform { get; private set; }
    public CardData CardData { get; private set; }
    public CardAnimator CardAnimator { get; private set; }
    public CardTiltSettings TiltSettings => tiltSettings;
    
    // Propriétés pour accéder aux états
    public CardIdleState IdleState => idleState;
    public CardHoverState HoverState => hoverState;
    public CardSelectedState SelectedState => selectedState;
    public CardDraggingState DraggingState => draggingState;

    private void Awake()
    {
        Transform = transform;
        CardData = GetComponent<CardData>();
        CardAnimator = GetComponent<CardAnimator>();
        
        // Initialiser les états
        idleState = new CardIdleState(this);
        hoverState = new CardHoverState(this);
        selectedState = new CardSelectedState(this);
        draggingState = new CardDraggingState(this);
        
        // Démarrer à l'état Idle
        currentState = idleState;
        currentState.OnEnter();
    }

    private void Update()
    {
        currentState?.OnUpdate();
    }

    public void ChangeState(ICardState newState)
    {
        if (currentState == newState) return;
        
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }

    public bool IsInState<T>() where T : ICardState
    {
        return currentState is T;
    }
}