using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// CARDDATA - Composant GameObject contenant les données runtime d'une carte
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Pont entre le modèle Card (données) et le GameObject Unity
/// - Gère les références visuelles (SpriteRenderer, Collider)
/// - Stocke l'état runtime (position initiale, sorting order)
/// 
/// 📦 RESPONSABILITÉS :
/// - CardInfo : Référence vers le modèle Card
/// - SetFrontSprite() / SetBackSprite() : Applique les sprites
/// - NormalizeSpriteSize() : Redimensionne pour garder proportions constantes
/// - UpdateColliderSize() : Ajuste le BoxCollider2D à la taille de la carte
/// - ShowFront() / ShowBack() : Affiche face/dos de la carte
/// 
/// 🔗 COMPOSANTS LIÉS :
/// - CardInteraction : Lit CardInfo pour les événements
/// - CardStateMachine : Accède aux SpriteRenderers pour le sorting order
/// - HandView : Initialise CardInfo et les sprites
/// 
/// 📊 DONNÉES STOCKÉES :
/// - frontSpriteRenderer / backSpriteRenderer : Affichage visuel
/// - boxCollider2D : Zone de détection souris
/// - positionInitiale / rotationInitiale : État de repos
/// - sortingOrderInitiale : Ordre d'affichage de base
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Ajouter des effets visuels (glow, outline, particles)
/// - Implémenter un système de rareté (couleur de bordure)
/// - Ajouter des animations de flip (retourner la carte)
/// - Afficher des statistiques sur la carte (mana, attack, defense)
/// - Créer des skins/thèmes de cartes
/// - Ajouter un système de wear & tear (usure visuelle)
/// 
/// ⚙️ CONFIGURATION INSPECTOR :
/// - frontSpriteRenderer : Sprite de la face avant
/// - backSpriteRenderer : Sprite du dos
/// - boxCollider2D : Collider pour l'interaction souris
/// - cardWidth / cardHeight : Taille normalisée des cartes
/// 
/// ═══════════════════════════════════════════════════════════════════════════
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