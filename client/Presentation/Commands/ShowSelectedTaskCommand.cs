using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class ShowSelectedTaskCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<ProjectTask, Task> _updateTaskAsync;
    private readonly Func<ProjectTask, Task> _deleteTaskAsync;

    public ShowSelectedTaskCommand(
        ILogger logger,
        Func<ProjectTask, Task> updateTaskAsync,
        Func<ProjectTask, Task> deleteTaskAsync
    )
    {
        _logger = logger;
        _updateTaskAsync = updateTaskAsync;
        _deleteTaskAsync = deleteTaskAsync;
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
                if (window.DeleteRequested)
                {
                    await _deleteTaskAsync(task);
                    return;
                }

                ProjectTask? updatedTask = vm.UpdateTask();
                if (updatedTask == null)
                    return;

                await _updateTaskAsync(updatedTask);
            }
            catch (Exception ex)
            {
                _logger.Log(client.Domain.Enum.LogLevel.ERROR, $"Failed To Save Task Changes: {ex.Message}");
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
