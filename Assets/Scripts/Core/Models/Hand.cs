using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// HAND - Modèle de données représentant la main du joueur
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Collection ordonnée de cartes (List<Card>)
/// - Gère l'ajout, la suppression et la réorganisation des cartes
/// - Couche "Modèle" dans le pattern MVC
/// 
/// 📦 RESPONSABILITÉS :
/// - AddCard() : Ajoute une carte à la fin de la main
/// - RemoveCard() : Retire une carte de la main
/// - ReorderCard() : Change la position d'une carte dans la liste
/// - Clear() : Vide complètement la main
/// 
/// 🔗 UTILISÉ PAR :
/// - HandController : Pour modifier la main
/// - DrawHandCommand : Pour piocher des cartes
/// - HandView : Pour afficher les cartes (lecture seule via Cards property)
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Ajouter des méthodes : Shuffle(), DrawTopCard(), InsertAt()
/// - Implémenter une limite de cartes maximum
/// - Ajouter des événements OnCardAdded/OnCardRemoved
/// - Sauvegarder/Charger l'état de la main
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class Hand : IHand
{
    private List<Card> cards = new List<Card>();

    public IReadOnlyList<Card> Cards => cards.AsReadOnly();
    public int Count => cards.Count;

    public void AddCard(Card card) => cards.Add(card);
    public void RemoveCard(Card card) => cards.Remove(card);
    public void Clear() => cards.Clear();
    
    /// <summary>
    /// Réorganise une carte à un nouvel index dans la main
    /// </summary>
    public void ReorderCard(Card card, int newIndex)
    {
        int currentIndex = cards.IndexOf(card);
        if (currentIndex == -1 || currentIndex == newIndex) return;
        
        cards.RemoveAt(currentIndex);
        newIndex = Mathf.Clamp(newIndex, 0, cards.Count);
        cards.Insert(newIndex, card);
    }
}
