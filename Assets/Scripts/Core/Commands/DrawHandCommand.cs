using System.Collections.Generic;

/// <summary>
/// Command pour piocher des cartes dans la main
/// </summary>
public class DrawHandCommand : ICommand
{
    private readonly Hand hand;
    private readonly HandView view;
    private readonly List<Card> cardsToAdd;
    private readonly List<Card> addedCards;

    public DrawHandCommand(Hand hand, HandView view, List<Card> cards)
    {
        this.hand = hand;
        this.view = view;
        this.cardsToAdd = cards;
        this.addedCards = new List<Card>();
    }

    public void Execute()
    {
        addedCards.Clear();
        
        foreach (Card card in cardsToAdd)
        {
            hand.AddCard(card);
            addedCards.Add(card);
        }
        
        view.UpdateDisplay(hand.Cards);
    }

    public void Undo()
    {
        foreach (Card card in addedCards)
        {
            hand.RemoveCard(card);
        }
        
        addedCards.Clear();
        view.UpdateDisplay(hand.Cards);
    }
}