// =============================================================================
// Author: Oliver
// =============================================================================

using client.Application.Interfaces;

namespace client.Presentation.Commands;

public class QuitCommand : IUndoableCommand
{
    // Author: Oliver
    public void Execute(object? param) => System.Windows.Application.Current.Shutdown();

    // Author: Oliver
    public void Undo() { }

    // Author: Oliver
    public bool CanExecute(object? param) => true;

    public event EventHandler? CanExecuteChanged;
}
