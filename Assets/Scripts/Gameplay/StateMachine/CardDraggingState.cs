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
    private Vector3 lastPosition;
    private Vector3 velocity;
    
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
            lastPosition = worldPosition + dragOffset;
            velocity = Vector3.zero;
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
            
            Vector3 targetPosition = worldPosition + dragOffset;
            
            // Calculer la vélocité pour l'effet de tilt
            velocity = (targetPosition - lastPosition) / Time.deltaTime;
            lastPosition = targetPosition;
            
            // Appliquer la position
            stateMachine.Transform.position = targetPosition;
            
            // Appliquer l'effet de rotation flottante
            ApplyTiltRotation();

            // Notifier le changement de position pour réorganiser la main
            if (CardEventBus.Instance != null)
            {
                CardEventBus.Instance.RaiseUpdateCardIndex(stateMachine.gameObject, targetPosition);
            }
        }
    }

    /// <summary>
    /// Applique une rotation dynamique basée sur la vélocité du mouvement
    /// </summary>
    private void ApplyTiltRotation()
    {
        // Récupérer les settings (ou utiliser valeurs par défaut si null)
        CardTiltSettings settings = stateMachine.TiltSettings;
        
        float tiltIntensityX = settings != null ? settings.tiltIntensityX : 15f;
        float tiltIntensityY = settings != null ? settings.tiltIntensityY : 15f;
        float tiltIntensityZ = settings != null ? settings.tiltIntensityZ : 20f;
        float tiltSmoothSpeed = settings != null ? settings.tiltSmoothSpeed : 8f;
        float maxTiltAngleXY = settings != null ? settings.maxTiltAngleXY : 30f;
        float maxTiltAngleZ = settings != null ? settings.maxTiltAngleZ : 45f;
        
        // Calculer les angles de rotation basés sur la vélocité
        float tiltX = Mathf.Clamp(-velocity.y * tiltIntensityX, -maxTiltAngleXY, maxTiltAngleXY);
        float tiltY = Mathf.Clamp(velocity.x * tiltIntensityY, -maxTiltAngleXY, maxTiltAngleXY);
        float tiltZ = Mathf.Clamp(-velocity.x * tiltIntensityZ, -maxTiltAngleZ, maxTiltAngleZ);
        
        // Créer la rotation cible
        Quaternion targetRotation = Quaternion.Euler(tiltX, tiltY, tiltZ);
        
        // Lisser la rotation pour un effet plus naturel
        stateMachine.Transform.rotation = Quaternion.Lerp(
            stateMachine.Transform.rotation,
            targetRotation,
            Time.deltaTime * tiltSmoothSpeed
        );
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