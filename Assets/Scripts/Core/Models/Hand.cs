using System.Collections.Generic;
using UnityEngine;

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
