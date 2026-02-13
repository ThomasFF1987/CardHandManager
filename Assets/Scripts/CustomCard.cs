using UnityEngine;

/// <summary>
/// Exemple d'implémentation personnalisée d'une carte
/// </summary>
public class CustomCard : ICard
{
    private Sprite cachedSprite;
    
    public string Id => "custom_001";
    public string Name => "Ma Carte Personnalisée";
    
    public Sprite GetVisual()
    {
        if (cachedSprite == null)
        {
            cachedSprite = Resources.Load<Sprite>("MySprite");
        }
        return cachedSprite;
    }
}
