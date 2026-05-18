using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class CreateNewTaskCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Action<ProjectTask> _onTaskCreated;
    private readonly Func<bool> _canCreateTask;

    public CreateNewTaskCommand(
        ILogger logger,
        Action<ProjectTask> onTaskCreated,
        Func<bool> canCreateTask
    )
    {
        _logger = logger;
        _onTaskCreated = onTaskCreated;
        _canCreateTask = canCreateTask;
    }

    public void Execute(object? param)
    {
        if (!CanExecute(param))
            return;

        var vm = new NewTaskViewModel(_logger);
        var window = new NewTaskWindow { DataContext = vm };

        if (window.ShowDialog() == true)
        {
            ProjectTask task = vm.CreateProjectTask();
            _onTaskCreated(task);
        }
    }

    public void Undo() { } // No undoing for now.

    public bool CanExecute(object? param) => _canCreateTask();

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
