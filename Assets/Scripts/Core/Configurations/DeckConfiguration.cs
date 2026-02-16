using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// DECKCONFIGURATION - ScriptableObject représentant un deck de cartes
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Configuration d'un deck complet (liste de CardConfiguration)
/// - Asset réutilisable dans l'Inspector Unity
/// - Pattern Strategy pour différents types de decks
/// 
/// 📦 RESPONSABILITÉS :
/// - Stocker la liste des cartes du deck
/// - Métadonnées du deck (nom, description, icône)
/// - Validation de la configuration
/// 
/// 🔗 UTILISÉ PAR :
/// - DeckManager : Charge le deck depuis ce ScriptableObject
/// - DeckBuilder : Construit des decks personnalisés
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Créer plusieurs decks (Starter, Advanced, Boss, etc.)
/// - Ajouter des règles de construction (min/max cartes)
/// - Implémenter des decks thématiques (Fire, Water, etc.)
/// - Ajouter des statistiques (winrate, difficulté)
/// 
/// 📂 CRÉATION :
/// Right-click in Project → Create → Card Game → Deck Configuration
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
[CreateAssetMenu(fileName = "New Deck", menuName = "CardHandManager ScriptableObjects/Deck Configuration", order = 1)]
public class DeckConfiguration : ScriptableObject
{
    [Header("Deck Info")]
    [Tooltip("Nom du deck (ex: Starter Deck, Fire Deck, Boss Deck)")]
    public string deckName = "New Deck";
    
    [Tooltip("Description du deck")]
    [TextArea(3, 6)]
    public string description = "Description du deck";
    
    [Tooltip("Icône du deck (optionnel)")]
    public Sprite deckIcon;

    [Header("Deck Cards")]
    [Tooltip("Liste des cartes dans ce deck")]
    public List<CardConfiguration> cards = new List<CardConfiguration>();

    [Header("Deck Rules")]
    [Tooltip("Nombre minimum de cartes dans le deck")]
    [Min(1)]
    public int minDeckSize = 30;
    
    [Tooltip("Nombre maximum de cartes dans le deck (0 = illimité)")]
    public int maxDeckSize = 60;
    
    [Tooltip("Mélanger automatiquement au chargement")]
    public bool shuffleOnLoad = true;

    /// <summary>
    /// Nombre total de cartes dans le deck
    /// </summary>
    public int CardCount => cards != null ? cards.Count : 0;

    /// <summary>
    /// Vérifie si le deck est valide
    /// </summary>
    public bool IsValid()
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning($"Deck '{deckName}' est vide !");
            return false;
        }

        if (cards.Count < minDeckSize)
        {
            Debug.LogWarning($"Deck '{deckName}' a seulement {cards.Count} cartes (min: {minDeckSize})");
            return false;
        }

        if (maxDeckSize > 0 && cards.Count > maxDeckSize)
        {
            Debug.LogWarning($"Deck '{deckName}' a {cards.Count} cartes (max: {maxDeckSize})");
            return false;
        }

        // Vérifier qu'il n'y a pas de null
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null)
            {
                Debug.LogWarning($"Deck '{deckName}' contient une carte null à l'index {i}");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Obtient une copie de la liste de cartes (pour éviter modifications)
    /// </summary>
    public List<CardConfiguration> GetCardsCopy()
    {
        return new List<CardConfiguration>(cards);
    }

    /// <summary>
    /// Debug info dans l'Inspector
    /// </summary>
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(deckName))
        {
            deckName = name;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Validate Deck")]
    private void ValidateDeck()
    {
        if (IsValid())
        {
            Debug.Log($"✅ Deck '{deckName}' est valide ({CardCount} cartes)");
        }
    }

    [ContextMenu("Log Deck Info")]
    private void LogDeckInfo()
    {
        Debug.Log($"=== Deck: {deckName} ===");
        Debug.Log($"Cards: {CardCount}");
        Debug.Log($"Shuffle on load: {shuffleOnLoad}");
        Debug.Log($"Description: {description}");
        
        if (cards != null)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] != null)
                {
                    Debug.Log($"  [{i}] {cards[i].cardName}");
                }
            }
        }
    }
#endif
}