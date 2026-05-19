using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class ShowSelectedTaskCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly ITaskService _taskService;
    private readonly Action<ProjectTask> _onTaskUpdated;

    public ShowSelectedTaskCommand(
        ILogger logger,
        ITaskService taskService,
        Action<ProjectTask> onTaskUpdated
    )
    {
        _logger = logger;
        _taskService = taskService;
        _onTaskUpdated = onTaskUpdated;
    }

    public async void Execute(object? param)
    {
        if (!CanExecute(param))
            return;

        var task = (ProjectTask)param!;
        var vm = new SelectedTaskViewModel(_logger, task);
        var window = new SelectedTaskWindow { DataContext = vm };

        if (window.ShowDialog() == true)
        {
            try
            {
                ProjectTask? updatedTask = vm.UpdateTask();
                if (updatedTask == null)
                    return;

                await _taskService.UpdateTaskAsync(updatedTask);
                _onTaskUpdated(updatedTask);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.ERROR, $"Failed To Update Task: {ex.Message}");
            }
        }
    }

    public void Undo() { }

    public bool CanExecute(object? param) => param is ProjectTask;

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
