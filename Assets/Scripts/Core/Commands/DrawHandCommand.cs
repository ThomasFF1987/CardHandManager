using System.Collections.Generic;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// DRAWHANDCOMMAND - Commande pour piocher des cartes dans la main
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Encapsule l'action de piocher plusieurs cartes
/// - Implémente le pattern Command pour permettre Undo/Redo
/// - Synchronise le modèle (Hand) avec la vue (HandView)
/// 
/// 📦 RESPONSABILITÉS :
/// - Execute() : Ajoute les cartes à la main et met à jour l'affichage
/// - Undo() : Retire les cartes ajoutées et restaure l'état précédent
/// - Garde la trace des cartes ajoutées pour l'annulation
/// 
/// 🔗 UTILISÉ PAR :
/// - HandController.DrawInitialHand() : Pioche la main de départ avec G
/// 
/// 📊 FLUX D'EXÉCUTION :
/// 1. Execute() appelé par le HandController
/// 2. Pour chaque carte → hand.AddCard()
/// 3. Mise à jour de l'affichage → view.UpdateDisplay()
/// 4. Stockage des cartes ajoutées dans addedCards (pour Undo)
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Ajouter une animation de pioche progressive
/// - Piocher depuis un Deck au lieu d'une liste
/// - Ajouter des effets sonores/visuels
/// - Vérifier si la main n'est pas pleine avant d'ajouter
/// - Logger les commandes pour débug/analytics
/// 
/// ⚙️ PARAMÈTRES :
/// - hand : Le modèle de la main (données)
/// - view : La vue de la main (affichage)
/// - cardsToAdd : Les cartes à piocher
/// 
/// ═══════════════════════════════════════════════════════════════════════════
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