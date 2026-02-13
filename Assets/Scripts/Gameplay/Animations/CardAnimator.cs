using System.Collections;
using UnityEngine;

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
        
        Vector3 hoverPosition = targetPosition + new Vector3(0, hoverHeight, 0);
        transform.localScale = initialScale * hoverScale;
        currentAnimation = StartCoroutine(MoveToPosition(hoverPosition, Quaternion.identity));
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
