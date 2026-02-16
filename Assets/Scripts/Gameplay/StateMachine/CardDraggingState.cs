using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// CARDDRAGGINGSTATE - État "Déplacement" de la carte
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - État actif pendant le drag & drop de la carte
/// - Suit la position de la souris en temps réel
/// - Applique une rotation "tilt" basée sur la vélocité
/// - Notifie le HandController pour réorganiser les cartes
/// 
/// 📦 RESPONSABILITÉS :
/// - OnEnter() : Calcule l'offset entre souris et carte
/// - OnUpdate() : 
///     1. Calcule position monde de la souris
///     2. Applique la position + offset
///     3. Calcule la vélocité
///     4. Applique le tilt rotation (effet inertie)
///     5. Notifie CardEventBus.RaiseUpdateCardIndex() (throttlé 50ms)
/// - OnExit() : Restaure le sorting order
/// 
/// 🎮 TILT ROTATION :
/// - Rotation X : Basée sur vélocité Y (carte penche en avant/arrière)
/// - Rotation Y : Basée sur vélocité X (carte penche gauche/droite)
/// - Rotation Z : Basée sur vélocité X (carte s'incline comme une ailette)
/// - Lerp smooth pour un effet fluide
/// 
/// 📊 OPTIMISATIONS :
/// - INDEX_UPDATE_INTERVAL : 50ms entre notifications (20 Hz)
/// - Throttling pour éviter de spammer le HandController
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Ajouter une traînée de particules pendant le drag
/// - Créer des zones de drop avec highlight
/// - Afficher un fantôme de la carte à sa future position
/// - Ajouter un système de snap-to-grid
/// - Implémenter un shake effect si drop invalide
/// - Créer des restrictions de drag (zones interdites)
/// - Ajouter un feedback sonore pendant le mouvement
/// 
/// ⚙️ CONFIGURATION :
/// Utilise CardTiltSettings pour :
/// - tiltIntensityX/Y/Z : Force de l'inclinaison
/// - maxTiltAngleXY/Z : Limite des rotations
/// - tiltSmoothSpeed : Vitesse du lerp
/// 
/// 📐 GESTION CAMÉRA :
/// Supporte Orthographic et Perspective avec calculs adaptés
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
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