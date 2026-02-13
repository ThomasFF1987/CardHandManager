using UnityEngine;

/// <summary>
/// Machine à états pour gérer les différents états d'une carte
/// </summary>
public class CardStateMachine : MonoBehaviour
{
    private ICardState currentState;
    
    // Références aux composants nécessaires
    public CardData CardData { get; private set; }
    public CardAnimator CardAnimator { get; private set; }
    public Transform Transform { get; private set; }
    
    // États disponibles
    public CardIdleState IdleState { get; private set; }
    public CardHoverState HoverState { get; private set; }
    public CardSelectedState SelectedState { get; private set; }
    public CardDraggingState DraggingState { get; private set; }
    
    private void Awake()
    {
        // Récupérer les références
        CardData = GetComponent<CardData>();
        CardAnimator = GetComponent<CardAnimator>();
        Transform = transform;
        
        // Initialiser les états
        IdleState = new CardIdleState(this);
        HoverState = new CardHoverState(this);
        SelectedState = new CardSelectedState(this);
        DraggingState = new CardDraggingState(this);
        
        // ✅ CORRECTION : Commencer à l'état Idle dès Awake
        ChangeState(IdleState);
    }

    private void Update()
    {
        currentState?.OnUpdate();
    }

    /// <summary>
    /// Change l'état actuel de la carte
    /// </summary>
    public void ChangeState(ICardState newState)
    {
        if (currentState == newState) return;
        
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
        
        #if UNITY_EDITOR
        Debug.Log($"[{gameObject.name}] État changé vers : {currentState?.StateName}");
        #endif
    }

    /// <summary>
    /// Obtient l'état actuel
    /// </summary>
    public ICardState GetCurrentState() => currentState;
    
    /// <summary>
    /// Vérifie si on est dans un état donné
    /// </summary>
    public bool IsInState<T>() where T : ICardState
    {
        return currentState is T;
    }
}