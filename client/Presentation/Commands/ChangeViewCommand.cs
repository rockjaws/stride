using client.Application.Interfaces;
using client.Presentation.ViewModels;
using System.Windows.Input;

namespace client.Presentation.Commands;

public class ChangeViewCommand : IUndoableCommand
{
    private readonly MainViewModel _mainViewModel;
    private object? _previousView;

    public ChangeViewCommand(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public void Execute(object? param)
    {
        if (param is null)
            return;

        _previousView = _mainViewModel.CurrentView;
        _mainViewModel.SetCurrentView(param);
    }

    public void Undo()
    {
        if (_previousView is not null)
            _mainViewModel.SetCurrentView(_previousView);
    }

    public bool CanExecute(object? param) => param is not null;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
