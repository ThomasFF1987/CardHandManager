using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// CARDLAYOUTCALCULATOR - Calcul de positions pour la réorganisation
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Algorithmes de calcul de layout (stateless)
/// - Détermine le nouvel index d'une carte pendant le drag
/// - Séparation logique pure / UI
/// 
/// 📦 RESPONSABILITÉS :
/// - CalculateCardIndex() : Index basé sur position mondiale
/// - IsPositionTooHigh() : Vérifie si carte au-dessus du seuil
/// 
/// 💡 AVANTAGES :
/// - Testable unitairement (pas de MonoBehaviour)
/// - Réutilisable pour d'autres layouts
/// - Respect du SRP
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public static class CardLayoutCalculator
{
    // ✨ Cache pour éviter GetComponent répétés
    private static readonly Dictionary<GameObject, CardData> cardDataCache = new Dictionary<GameObject, CardData>();
    
    public static int CalculateCardIndex(Vector3 worldPosition, Hand hand, HandView view)
    {
        if (hand.Count == 0) return 0;

        float minDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand.Cards[i];
            GameObject cardGO = view.GetCardGameObject(card);
            
            if (cardGO != null)
            {
                CardData cardData = GetOrCacheCardData(cardGO);
                if (cardData != null)
                {
                    float distance = Mathf.Abs(cardData.positionInitiale.x - worldPosition.x);
                    
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestIndex = i;
                    }
                }
            }
        }

        return closestIndex;
    }
    
    public static bool IsPositionTooHigh(Vector3 worldPosition, Vector3 initialPosition, float maxHeightOffset)
    {
        return (worldPosition.y - initialPosition.y) > maxHeightOffset;
    }
    
    // ✨ Méthode helper pour cache
    private static CardData GetOrCacheCardData(GameObject cardGO)
    {
        if (!cardDataCache.TryGetValue(cardGO, out CardData cardData))
        {
            cardData = cardGO.GetComponent<CardData>();
            if (cardData != null)
            {
                cardDataCache[cardGO] = cardData;
            }
        }
        return cardData;
    }
    
    // ✨ Pour nettoyer le cache si nécessaire
    public static void ClearCache()
    {
        cardDataCache.Clear();
    }
}