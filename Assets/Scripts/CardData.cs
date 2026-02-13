using UnityEngine;

/// <summary>
/// Composant contenant les données et références visuelles d'une carte
/// </summary>
public class CardData : MonoBehaviour
{
    [Header("Card Model")]
    public Card CardInfo { get; set; }

    [Header("Visual References")]
    public SpriteRenderer frontSpriteRenderer;
    public SpriteRenderer backSpriteRenderer;
    public BoxCollider2D boxCollider2D;

    [Header("State")]
    public Vector3 positionInitiale;
    public Quaternion rotationInitiale;
    public int sortingOrderInitiale;

    private void Awake()
    {
        if (boxCollider2D == null)
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
        }
    }

    /// <summary>
    /// Affiche le front de la carte
    /// </summary>
    public void ShowFront()
    {
        if (frontSpriteRenderer != null)
        {
            frontSpriteRenderer.enabled = true;
        }
        if (backSpriteRenderer != null)
        {
            backSpriteRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Affiche le dos de la carte
    /// </summary>
    public void ShowBack()
    {
        if (frontSpriteRenderer != null)
        {
            frontSpriteRenderer.enabled = false;
        }
        if (backSpriteRenderer != null)
        {
            backSpriteRenderer.enabled = true;
        }
    }

    /// <summary>
    /// Active ou désactive le collider de la carte
    /// </summary>
    public void SetColliderEnabled(bool enabled)
    {
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = enabled;
        }
    }
}