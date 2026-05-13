using System.Windows.Input;

namespace client.Application.Interfaces;

public interface IUndoableCommand : ICommand
{
    void Undo();
}
