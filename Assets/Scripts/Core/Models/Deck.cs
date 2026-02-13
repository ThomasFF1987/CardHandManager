using System.Collections.Generic;

public class Deck
{
    private List<Card> cards = new List<Card>();

    public IReadOnlyList<Card> Cards => cards.AsReadOnly();

    public void AddCard(Card card) => cards.Add(card);

    public Card DrawCard()
    {
        if (cards.Count == 0) return null;
        var card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public void Shuffle()
    {
        System.Random rng = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (cards[k], cards[n]) = (cards[n], cards[k]);
        }
    }
}
