using System.Collections;
using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// CARDANIMATOR - Gestionnaire des animations de carte
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Anime les transitions de position, rotation et scale
/// - Centralisé pour toutes les animations de carte
/// - Utilise des Coroutines avec Lerp smooth
/// 
/// 📦 RESPONSABILITÉS :
/// - SetTargetTransform() : Définit la position/rotation cible (layout)
/// - AnimateToTargetPosition() : Anime vers la position cible
/// - AnimateHover() : Monte la carte selon son axe Y local + scale
/// - AnimateUnhover() : Retour à la position normale
/// - AnimateSelected() : Scale up + rotation à 0
/// - AnimateDeselected() : Retour à l'état normal
/// 
/// 🎨 PARAMÈTRES ANIMABLES :
/// - hoverHeight : Hauteur de montée au survol (1f par défaut)
/// - hoverScale : Facteur de scale au survol (1.1x)
/// - selectedScale : Facteur de scale en sélection (1.2x)
/// - transitionSpeed : Vitesse du Lerp (2f par défaut)
/// 
/// 📊 GESTION COROUTINES :
/// - currentAnimation : Référence à la coroutine active
/// - StopCurrentAnimation() : Arrête la coroutine précédente
/// - MoveToPosition() : Coroutine générique position + rotation
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Ajouter des easing curves (EaseInOut, Bounce)
/// - Créer des animations de flip (retournement)
/// - Ajouter des shake effects
/// - Implémenter des animations de spawn (apparition)
/// - Créer des animations de destruction (disparition)
/// - Ajouter des trails/motion blur
/// - Synchroniser avec des effets sonores
/// 
/// ⚙️ ASTUCE AXE LOCAL :
/// AnimateHover() utilise transform.up (axe Y local) pour que
/// les cartes inclinées montent selon leur orientation
/// 
/// 🔄 CYCLE DE VIE ANIMATION :
/// 1. State change → AnimateXXX()
/// 2. StopCurrentAnimation() → Arrête l'ancienne
/// 3. StartCoroutine(MoveToPosition()) → Démarre la nouvelle
/// 4. Lerp jusqu'à atteindre la cible
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class CardAnimator : MonoBehaviour
{
    [SerializeField] private float hoverHeight = 1f;
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float selectedScale = 1.2f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 initialScale;
    private Coroutine currentAnimation;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    /// <summary>
    /// Définit la position cible que la carte doit atteindre
    /// </summary>
    public void SetTargetTransform(Vector3 position, Quaternion rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
    }

    /// <summary>
    /// Anime la carte vers sa position cible
    /// </summary>
    public void AnimateToTargetPosition()
    {
        StopCurrentAnimation();
        currentAnimation = StartCoroutine(MoveToPosition(targetPosition, targetRotation));
    }

    public void AnimateHover()
    {
        StopCurrentAnimation();
        
        // Utiliser l'axe Y local (transform.up) au lieu de l'axe Y global (Vector3.up)
        Vector3 hoverPosition = targetPosition + (transform.up * hoverHeight);
        transform.localScale = initialScale * hoverScale;
        currentAnimation = StartCoroutine(MoveToPosition(hoverPosition, targetRotation));
    }

    public void AnimateUnhover()
    {
        StopCurrentAnimation();
        
        transform.localScale = initialScale;
        currentAnimation = StartCoroutine(MoveToPosition(targetPosition, targetRotation));
    }

    public void AnimateSelected()
    {
        StopCurrentAnimation();
        
        transform.localScale = initialScale * selectedScale;
        transform.rotation = Quaternion.identity;
    }

    public void AnimateDeselected()
    {
        ResetScale();
        
        StopCurrentAnimation();
        currentAnimation = StartCoroutine(MoveToPosition(targetPosition, targetRotation));
    }

    public void ResetScale()
    {
        transform.localScale = initialScale;
    }

    /// <summary>
    /// Arrête toutes les animations en cours
    /// </summary>
    public void StopAllAnimations()
    {
        StopCurrentAnimation();
    }

    private void StopCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
    }

    private IEnumerator MoveToPosition(Vector3 target, Quaternion rotation)
    {
        while (Vector3.Distance(transform.localPosition, target) > 0.01f ||
               Quaternion.Angle(transform.localRotation, rotation) > 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * transitionSpeed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, Time.deltaTime * transitionSpeed);
            yield return null;
        }

        transform.localPosition = target;
        transform.localRotation = rotation;
        currentAnimation = null;
    }
}
