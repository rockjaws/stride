// =============================================================================
// Author: Oliver
// =============================================================================

using System.Windows.Input;

namespace client.Application.Interfaces;

public interface IUndoableCommand : ICommand
{
    // Author: Oliver
    void Undo();
}
