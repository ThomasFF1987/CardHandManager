using System.Collections.Generic;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// DRAWHANDCOMMAND - Commande pour piocher des cartes du deck vers la main
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Encapsule l'action de piocher plusieurs cartes depuis le DeckManager
/// - Implémente le pattern Command pour permettre Undo/Redo
/// - Synchronise le modèle (Hand) avec la vue (HandView)
/// 
/// 📦 RESPONSABILITÉS :
/// - Execute() : Pioche cartes du deck, ajoute à la main, met à jour l'affichage
/// - Undo() : Retire les cartes de la main et les remet dans le deck
/// - Garde la trace des cartes piochées pour l'annulation
/// 
/// 🔗 UTILISÉ PAR :
/// - HandController.DrawInitialHand() : Pioche la main de départ avec G
/// 
/// 📊 FLUX D'EXÉCUTION :
/// 1. Execute() appelé par le HandController
/// 2. DeckManager.DrawCards(count) → Obtient les cartes
/// 3. Pour chaque carte → hand.AddCard()
/// 4. Mise à jour de l'affichage → view.UpdateDisplay()
/// 5. Stockage des cartes piochées dans drawnCards (pour Undo)
/// 
/// 💡 AMÉLIORATIONS POSSIBLES :
/// - Ajouter une animation de pioche progressive
/// - Vérifier si la main n'est pas pleine avant d'ajouter
/// - Logger les commandes pour débug/analytics
/// - Gérer le cas où le deck est vide (piocher depuis la défausse)
/// 
/// ⚙️ PARAMÈTRES :
/// - deckManager : Gère le deck de cartes
/// - hand : Le modèle de la main (données)
/// - view : La vue de la main (affichage)
/// - cardsToDraw : Nombre de cartes à piocher
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class DrawHandCommand : ICommand
{
    private readonly DeckManager deckManager;
    private readonly Hand hand;
    private readonly HandView view;
    private readonly int cardsToDraw;
    private readonly List<Card> drawnCards;
    private readonly bool useRandomDraw;

    /// <summary>
    /// Constructeur pour piocher depuis le deck
    /// </summary>
    public DrawHandCommand(DeckManager deckManager, Hand hand, HandView view, int cardsToDraw)
    {
        this.deckManager = deckManager;
        this.hand = hand;
        this.view = view;
        this.cardsToDraw = cardsToDraw;
        this.drawnCards = new List<Card>();
        this.useRandomDraw = false;
    }

    /// <summary>
    /// Constructeur pour piocher des cartes aléatoires (pour debug/testing)
    /// </summary>
    public DrawHandCommand(DeckManager deckManager, Hand hand, HandView view, int cardsToDraw, bool randomDraw)
    {
        this.deckManager = deckManager;
        this.hand = hand;
        this.view = view;
        this.cardsToDraw = cardsToDraw;
        this.drawnCards = new List<Card>();
        this.useRandomDraw = randomDraw;
    }

    public void Execute()
    {
        drawnCards.Clear();
        
        // Piocher les cartes depuis le DeckManager
        List<Card> cards = useRandomDraw 
            ? deckManager.DrawRandomCards(cardsToDraw)
            : deckManager.DrawCards(cardsToDraw);
        
        // Ajouter les cartes piochées à la main
        foreach (Card card in cards)
        {
            hand.AddCard(card);
            drawnCards.Add(card);
        }
        
        view.UpdateDisplay(hand.Cards);
    }

    public void Undo()
    {
        // Retirer les cartes de la main
        foreach (Card card in drawnCards)
        {
            hand.RemoveCard(card);
            
            // Remettre la carte dans le deck (optionnel)
            if (!useRandomDraw)
            {
                deckManager.AddCardToDeck(card);
            }
        }
        
        drawnCards.Clear();
        view.UpdateDisplay(hand.Cards);
    }
}