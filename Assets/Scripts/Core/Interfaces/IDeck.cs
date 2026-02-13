public interface IDeck
{
    void AddCard(ICard card);
    ICard DrawCard();
    void Shuffle();
    int CardCount { get; }
}
