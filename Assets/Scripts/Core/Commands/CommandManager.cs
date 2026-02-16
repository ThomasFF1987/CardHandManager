using System.Collections.Generic;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// COMMANDMANAGER - Gestionnaire de commandes avec Undo/Redo
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Historique des commandes exécutées
/// - Système Undo/Redo avec Stack
/// - Pattern Command + Memento
/// 
/// 📦 RESPONSABILITÉS :
/// - ExecuteCommand() : Exécute et stocke dans l'historique
/// - Undo() : Annule la dernière commande
/// - Redo() : Réexécute une commande annulée
/// - Clear() : Vide l'historique
/// 
/// 💡 UTILISATION :
/// commandManager.ExecuteCommand(new DrawHandCommand(...));
/// commandManager.Undo(); // Annule la pioche
/// commandManager.Redo(); // Repioche
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class CommandManager
{
    private readonly Stack<ICommand> undoStack = new Stack<ICommand>();
    private readonly Stack<ICommand> redoStack = new Stack<ICommand>();
    
    public bool CanUndo => undoStack.Count > 0;
    public bool CanRedo => redoStack.Count > 0;
    
    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear(); // Reset redo stack après nouvelle action
    }
    
    public void Undo()
    {
        if (!CanUndo) return;
        
        ICommand command = undoStack.Pop();
        command.Undo();
        redoStack.Push(command);
    }
    
    public void Redo()
    {
        if (!CanRedo) return;
        
        ICommand command = redoStack.Pop();
        command.Execute();
        undoStack.Push(command);
    }
    
    public void Clear()
    {
        undoStack.Clear();
        redoStack.Clear();
    }
}