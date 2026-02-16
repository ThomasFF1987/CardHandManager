using System.Collections.Generic;

public interface IDeck
{
    IReadOnlyList<Card> Cards { get; }
    int Count { get; }
    
    void AddCard(Card card);
    Card DrawCard();
    void Shuffle();
}
