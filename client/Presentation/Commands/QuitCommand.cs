using client.Application.Interfaces;

namespace client.Presentation.Commands;

public class QuitCommand : IUndoableCommand
{
    public void Execute(object? param) => System.Windows.Application.Current.Shutdown();

    public void Undo() { } // No undoing for now.

    public bool CanExecute(object? param) => true; // Some logic for unsaved work maybe?

    public event EventHandler? CanExecuteChanged;
}
