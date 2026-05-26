using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class SelectedTaskViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly ProjectTask _originalTask;
    private readonly IUserService _userService;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private DateTime _startDate = DateTime.Today;
    private DateTime _deadline = DateTime.Today;
    private TaskProgress _progress = TaskProgress.Backlog;
    private TaskPriority _priority = TaskPriority.Normal;
    private List<User>? _memberOptions;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime Deadline
    {
        get => _deadline;
        set => SetProperty(ref _deadline, value);
    }

    public TaskProgress Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    public TaskPriority Priority
    {
        get => _priority;
        set => SetProperty(ref _priority, value);
    }

    public TaskProgress[] ProgressOptions { get; } = Enum.GetValues<TaskProgress>();

    public TaskPriority[] PriorityOptions { get; } = Enum.GetValues<TaskPriority>();

    public List<User>? MemberOptions
    {
        get => _memberOptions;
        set => SetProperty(ref _memberOptions, value);
    }

    public SelectedTaskViewModel(ILogger logger, ProjectTask task, IUserService userService)
    {
        _logger = logger;
        _originalTask = task;
        _userService = userService;
        _title = task.Title;
        _description = task.Description;
        _startDate = task.StartDate;
        _deadline = task.Deadline;
        _progress = task.Progress;
        _priority = task.Priority;
        _ = GetUsersAsync(task.ProjectId);
    }

    private async Task GetUsersAsync(int? projectId)
    {
        MemberOptions = projectId.HasValue
            ? await _userService.GetUsersAsync(projectId.Value)
            : await _userService.GetUsersAsync();
    }

    public ProjectTask? UpdateTask()
    {
        if (_originalTask.Id == null)
        {
            _logger.Log(LogLevel.ERROR, "Cannot update a task before it has been saved.");
            return null;
        }

        _logger.Log(LogLevel.INFO, $"Prepared Task Update: {_originalTask.Id}");

        // Build a replacement task instead of mutating the original bound object before the user saves.
        return new ProjectTask(
            _originalTask.Id,
            _title,
            _description,
            _startDate,
            _deadline,
            _progress,
            _priority,
            _originalTask.ProjectId
        );
    }
}
