using System.Collections.ObjectModel;
using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class TaskViewModel : ObservableObject
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

    public TaskViewModel(ILogger logger, ITaskService taskService, IUserService userService)
    {
        _logger = logger;
        _taskService = taskService;
        _userService = userService;
        Tasks = [];

        _ = LoadTasksAsync();
    }

    public async Task LoadTasksAsync()
    {
        try
        {
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
}
