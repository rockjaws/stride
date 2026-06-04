using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class CreateNewTaskCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private readonly Func<ProjectTask, Task> _createTaskAsync;
    private readonly Func<bool> _canCreateTask;
    private readonly Func<int?> _getProjectId;

    public CreateNewTaskCommand(
        ILogger logger,
        IUserService userService,
        Func<ProjectTask, Task> createTaskAsync,
        Func<bool> canCreateTask,
        Func<int?> getProjectId
    )
    {
        _logger = logger;
        _userService = userService;
        _createTaskAsync = createTaskAsync;
        _canCreateTask = canCreateTask;
        _getProjectId = getProjectId;
    }

    public async void Execute(object? param)
    {
        if (!CanExecute(param))
            return;

        try
        {
            int projectId =
                _getProjectId()
                ?? throw new InvalidOperationException(
                    "Cannot create a task without a selected project."
                );

            var vm = new NewTaskViewModel(_logger, _userService, projectId);
            var window = new NewTaskWindow { DataContext = vm };

            if (window.ShowDialog() == true)
            {
                // The command owns window flow; the view model/service owns the actual create behavior.
                ProjectTask task = vm.CreateProjectTask(projectId);
                await _createTaskAsync(task);
            }
        }
        catch (Exception ex)
        {
            _logger.Log(client.Domain.Enum.LogLevel.ERROR, $"Failed To Create Task: {ex.Message}");
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
