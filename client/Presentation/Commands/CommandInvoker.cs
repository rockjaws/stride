// =============================================================================
// Author: Oliver
// =============================================================================

using client.Application.Interfaces;

namespace client.Presentation.Commands
{
    public class CommandInvoker
    {
        private readonly Stack<IUndoableCommand> _history = new();
        private readonly Stack<IUndoableCommand> _redoStack = new();

        // Author: Oliver
        public void Execute(IUndoableCommand command)
        {
            command.Execute(null);
            _history.Push(command);
            _redoStack.Clear();
        }

        // Author: Oliver
        public void Undo()
        {
            if (_history.TryPop(out var command))
            {
                command.Undo();
                _redoStack.Push(command);
            }
        }

        // Author: Oliver
        public void Redo()
        {
            if (_redoStack.TryPop(out var command))
            {
                command.Execute(null);
                _history.Push(command);
            }
        }
    }
}
