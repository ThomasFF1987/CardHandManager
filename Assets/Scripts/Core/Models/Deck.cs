using System.Collections.Generic;

public class Deck : IDeck
{
    private List<Card> cards = new List<Card>();
    private System.Random rng = new System.Random(); // ✨ Réutiliser l'instance

    public IReadOnlyList<Card> Cards => cards.AsReadOnly();
    public int Count => cards.Count;

    public void AddCard(Card card) => cards.Add(card);
    
    /// <summary>
    /// Ajoute une carte en haut du deck (pour Undo)
    /// </summary>
    public void AddCardToTop(Card card)
    {
        cards.Insert(0, card);
    }

    public Card DrawCard()
    {
        if (cards.Count == 0) return null;
        
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public void Shuffle()
    {
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (cards[k], cards[n]) = (cards[n], cards[k]); // ✅ Tuple deconstruction (C# 7.0+)
        }
    }
}
