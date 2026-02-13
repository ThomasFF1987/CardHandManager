using System.Collections.Generic;

public class Hand : IHand
{
    private List<Card> cards = new List<Card>();

    public IReadOnlyList<Card> Cards => cards.AsReadOnly();
    public int Count => cards.Count;

    public void AddCard(Card card) => cards.Add(card);
    public void RemoveCard(Card card) => cards.Remove(card);
    public void Clear() => cards.Clear();
}
