using UnityEngine;

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