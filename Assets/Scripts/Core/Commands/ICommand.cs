/// <summary>
/// Interface pour le pattern Command
/// </summary>
public interface ICommand
{
    void Execute();
    void Undo();
}