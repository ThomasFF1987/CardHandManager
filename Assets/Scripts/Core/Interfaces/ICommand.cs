/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// ICOMMAND - Interface pour le pattern Command
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Définit le contrat pour toutes les commandes du jeu
/// - Permet d'encapsuler des actions en objets
/// - Pattern de conception comportemental
/// 
/// 📦 RESPONSABILITÉS :
/// - Execute() : Exécute l'action de la commande
/// - Undo() : Annule l'action (retour arrière)
/// 
/// 🔗 IMPLÉMENTATIONS :
/// - DrawHandCommand : Piocher des cartes
/// - (Futures) PlayCardCommand, DiscardCardCommand, ShuffleCommand...
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Créer un système Undo/Redo avec une Stack<ICommand>
/// - Enregistrer l'historique des commandes pour replay
/// - Créer des macro-commandes (CompositeCommand)
/// - Sérialiser les commandes pour networking/save
/// 
/// 📚 PATTERN :
/// Command Pattern - Encapsule une requête comme un objet, permettant
/// de paramétrer des clients avec des requêtes différentes, de mettre
/// en file ou d'annuler des opérations.
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public interface ICommand
{
    void Execute();
    void Undo();
}