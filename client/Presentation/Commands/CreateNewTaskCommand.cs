using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class CreateNewTaskCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly ITaskService _taskService;
    private readonly Action<ProjectTask> _onTaskCreated;
    private readonly Func<bool> _canCreateTask;
    private readonly Func<int?> _getProjectId;

    public CreateNewTaskCommand(
        ILogger logger,
        ITaskService taskService,
        Action<ProjectTask> onTaskCreated,
        Func<bool> canCreateTask,
        Func<int?> getProjectId
    )
    {
        _logger = logger;
        _taskService = taskService;
        _onTaskCreated = onTaskCreated;
        _canCreateTask = canCreateTask;
        _getProjectId = getProjectId;
    }

    public async void Execute(object? param)
    {
        if (!CanExecute(param))
            return;

        var vm = new NewTaskViewModel(_logger);
        var window = new NewTaskWindow { DataContext = vm };

        if (window.ShowDialog() == true)
        {
            try
            {
                int projectId = _getProjectId()
                    ?? throw new InvalidOperationException("Cannot create a task without a selected project.");
                ProjectTask task = vm.CreateProjectTask(projectId);
                ProjectTask savedTask = await _taskService.CreateTaskAsync(task);
                _onTaskCreated(savedTask);
            }
            catch (Exception ex)
            {
                _logger.Log(Domain.Enum.LogLevel.ERROR, $"Failed To Create Task: {ex.Message}");
            }
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
