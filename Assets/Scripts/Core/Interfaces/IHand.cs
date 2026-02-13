using System.Collections.Generic;

public interface IHand
{
    IReadOnlyList<Card> Cards { get; }
    int Count { get; }

    void AddCard(Card card);
    void RemoveCard(Card card);
    void Clear();
}
