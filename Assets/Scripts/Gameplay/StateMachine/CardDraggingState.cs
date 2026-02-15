using UnityEngine;
using UnityEngine.InputSystem;

public class CardDraggingState : ICardState
{
    private readonly CardStateMachine stateMachine;
    private Camera mainCamera;
    private Vector3 dragOffset;
    private Vector3 lastPosition;
    private Vector3 velocity;
    
    // Throttling pour les mises à jour de position
    private float lastIndexUpdateTime;
    private const float INDEX_UPDATE_INTERVAL = 0.05f; // 20 fois par seconde max
    
    public string StateName => "Dragging";

    public CardDraggingState(CardStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        mainCamera = Camera.main;
        lastIndexUpdateTime = 0f;
        
        if (mainCamera != null && Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            
            Vector3 worldPosition;
            if (mainCamera.orthographic)
            {
                worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    Mathf.Abs(mainCamera.transform.position.z)
                ));
            }
            else
            {
                // En Perspective : utiliser la distance jusqu'à la carte
                float distanceToCard = mainCamera.transform.position.z - stateMachine.Transform.position.z;
                worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    Mathf.Abs(distanceToCard)
                ));
            }
            
            dragOffset = stateMachine.Transform.position - worldPosition;
            lastPosition = worldPosition + dragOffset;
            velocity = Vector3.zero;
        }
        
        if (stateMachine.CardAnimator != null)
        {
            stateMachine.CardAnimator.StopAllAnimations();
        }
    }

    public void OnUpdate()
    {
        if (mainCamera != null && Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            
            Vector3 worldPosition;
            if (mainCamera.orthographic)
            {
                worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    Mathf.Abs(mainCamera.transform.position.z)
                ));
            }
            else
            {
                // En Perspective : utiliser la distance jusqu'à la carte
                float distanceToCard = mainCamera.transform.position.z - stateMachine.Transform.position.z;
                worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    Mathf.Abs(distanceToCard)
                ));
            }
            
            Vector3 targetPosition = worldPosition + dragOffset;
            
            // Calculer la vélocité
            float deltaTime = Time.deltaTime;
            if (deltaTime > 0)
            {
                velocity = (targetPosition - lastPosition) / deltaTime;
            }
            lastPosition = targetPosition;
            
            // Appliquer la position
            stateMachine.Transform.position = targetPosition;
            
            // Appliquer l'effet de rotation
            ApplyTiltRotation();

            // Throttle les mises à jour d'index pour éviter de recalculer trop souvent
            if (Time.time - lastIndexUpdateTime >= INDEX_UPDATE_INTERVAL)
            {
                lastIndexUpdateTime = Time.time;
                
                if (CardEventBus.Instance != null)
                {
                    CardEventBus.Instance.RaiseUpdateCardIndex(stateMachine.gameObject, targetPosition);
                }
            }
        }
    }

    private void ApplyTiltRotation()
    {
        CardTiltSettings settings = stateMachine.TiltSettings;
        
        float tiltIntensityX = settings != null ? settings.tiltIntensityX : 15f;
        float tiltIntensityY = settings != null ? settings.tiltIntensityY : 15f;
        float tiltIntensityZ = settings != null ? settings.tiltIntensityZ : 20f;
        float tiltSmoothSpeed = settings != null ? settings.tiltSmoothSpeed : 8f;
        float maxTiltAngleXY = settings != null ? settings.maxTiltAngleXY : 30f;
        float maxTiltAngleZ = settings != null ? settings.maxTiltAngleZ : 45f;
        
        float tiltX = Mathf.Clamp(-velocity.y * tiltIntensityX, -maxTiltAngleXY, maxTiltAngleXY);
        float tiltY = Mathf.Clamp(velocity.x * tiltIntensityY, -maxTiltAngleXY, maxTiltAngleXY);
        float tiltZ = Mathf.Clamp(-velocity.x * tiltIntensityZ, -maxTiltAngleZ, maxTiltAngleZ);
        
        Quaternion targetRotation = Quaternion.Euler(tiltX, tiltY, tiltZ);
        
        stateMachine.Transform.rotation = Quaternion.Lerp(
            stateMachine.Transform.rotation,
            targetRotation,
            Time.deltaTime * tiltSmoothSpeed
        );
    }

    public void OnExit()
    {
        if (stateMachine.CardData != null)
        {
            stateMachine.CardData.frontSpriteRenderer.sortingOrder = stateMachine.CardData.sortingOrderInitiale;
            stateMachine.CardData.backSpriteRenderer.sortingOrder = stateMachine.CardData.sortingOrderInitiale;
        }
    }
}