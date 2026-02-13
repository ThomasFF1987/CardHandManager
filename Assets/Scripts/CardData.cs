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

    [Header("Sprite Size")]
    [SerializeField] private float cardWidth = 2.5f;
    [SerializeField] private float cardHeight = 3.5f;

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
    /// Applique le sprite front et normalise sa taille
    /// </summary>
    public void SetFrontSprite(Sprite sprite)
    {
        if (frontSpriteRenderer != null && sprite != null)
        {
            frontSpriteRenderer.sprite = sprite;
            NormalizeSpriteSize(frontSpriteRenderer);
            UpdateColliderSize();
        }
    }

    /// <summary>
    /// Applique le sprite back et normalise sa taille
    /// </summary>
    public void SetBackSprite(Sprite sprite)
    {
        if (backSpriteRenderer != null && sprite != null)
        {
            backSpriteRenderer.sprite = sprite;
            NormalizeSpriteSize(backSpriteRenderer);
            UpdateColliderSize();
        }
    }

    /// <summary>
    /// Normalise la taille du sprite pour qu'il garde des proportions constantes
    /// </summary>
    private void NormalizeSpriteSize(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;
        
        Sprite sprite = spriteRenderer.sprite;
        Vector2 spriteSize = sprite.bounds.size;
        
        float scaleX = cardWidth / spriteSize.x;
        float scaleY = cardHeight / spriteSize.y;
        
        spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    /// <summary>
    /// Met à jour la taille du BoxCollider2D pour correspondre à la taille normalisée de la carte
    /// </summary>
    private void UpdateColliderSize()
    {
        if (boxCollider2D != null)
        {
            boxCollider2D.size = new Vector2(cardWidth, cardHeight);
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