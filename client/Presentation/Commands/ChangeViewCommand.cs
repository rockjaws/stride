using client.Application.Interfaces;
using client.Domain.Enum;
using client.Presentation.ViewModels;
using System.Windows.Input;

namespace client.Presentation.Commands;

public class ChangeViewCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly MainViewModel _mainViewModel;
    private object? _previousView;

    public ChangeViewCommand(ILogger logger, MainViewModel mainViewModel)
    {
        _logger = logger;
        _mainViewModel = mainViewModel;
    }

    public void Execute(object? param)
    {
        if (param is null)
        {
            _logger.Log(LogLevel.WARNING, "ChangeViewCommand executed without a target view.");
            return;
        }

        _previousView = _mainViewModel.CurrentView;
        _mainViewModel.SetCurrentView(param);
        _logger.Log(LogLevel.INFO, $"Changed View To {param.GetType().Name}");
    }

    public void Undo()
    {
        if (_previousView is not null)
        {
            _mainViewModel.SetCurrentView(_previousView);
            _logger.Log(LogLevel.INFO, $"Restored View To {_previousView.GetType().Name}");
        }
    }

    public bool CanExecute(object? param) => param is not null;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
