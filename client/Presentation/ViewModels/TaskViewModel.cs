// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using System.Collections.ObjectModel;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class TaskViewModel : ObservableObject, IDisposable
{
    private readonly ILogger _logger;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    private ObservableCollection<ProjectTask> _tasks = [];

    public ObservableCollection<ProjectTask> Tasks
    {
        get => _tasks;
        set => SetProperty(ref _tasks, value);
    }

    public ShowSelectedTaskCommand ShowSelectedTaskCommand { get; }

    // Author: Nicolai and Oliver
    public TaskViewModel(ILogger logger, ITaskService taskService, IUserService userService)
    {
        _logger = logger;
        _taskService = taskService;
        _taskService.TasksChanged += OnGlobalStateChange;
        _userService = userService;
        Tasks = [];

        ShowSelectedTaskCommand = new ShowSelectedTaskCommand(
            _logger,
            _userService,
            UpdateTaskAsync,
            DeleteTaskAsync
        );

        _ = LoadTasksAsync();
    }

    // Author: Oliver
    public async Task LoadTasksAsync()
    {
        try
        {
            // The Tasks tab shows direct task assignments for the active user only.
            var tasks = await _taskService.GetTasksAsync(_userService.Id);

            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }

            _logger.Log(LogLevel.INFO, $"Loaded {Tasks.Count} Tasks For User {_userService.Id}");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Load Tasks For User {_userService.Id}: {ex.Message}");
        }
    }

    // Author: Nicolai and Oliver
    private async Task UpdateTaskAsync(ProjectTask task)
    {
        try
        {
            await _taskService.UpdateTaskAsync(task);
            ReplaceTask(task);
            _logger.Log(LogLevel.INFO, $"Updated Task {task.Id} From Tasks View");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Update Task {task.Id} From Tasks View: {ex.Message}");
        }
    }

    // Author: Nicolai and Oliver
    private async Task DeleteTaskAsync(ProjectTask task)
    {
        if (task.Id is not int id)
            return;

        try
        {
            await _taskService.DeleteTaskAsync(id);
            RemoveTask(task);
            _logger.Log(LogLevel.INFO, $"Deleted Task {id} From Tasks View");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Delete Task {id} From Tasks View: {ex.Message}");
        }
    }

    // Author: Oliver
    private void ReplaceTask(ProjectTask task)
    {
        var existingTask = Tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existingTask == null)
            return;

        // Replacing the collection item forces WPF to redraw badges and labels bound to the task.
        int taskIndex = Tasks.IndexOf(existingTask);
        Tasks[taskIndex] = task;
    }

    // Author: Oliver
    private void RemoveTask(ProjectTask task)
    {
        var existingTask = Tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existingTask != null)
            Tasks.Remove(existingTask);
    }

    // Author: Nicolai
    public void OnGlobalStateChange(object? sender, EventArgs e)
    {
        _ = LoadTasksAsync();
    }

    // Author: Nicolai
    public void Dispose()
    {
        _taskService.TasksChanged -= OnGlobalStateChange;
        GC.SuppressFinalize(this);
    }
}
