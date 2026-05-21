using System.Collections.ObjectModel;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Presentation.Algorithms;
using client.Domain.Models;
using client.Presentation.Common;
using client.Presentation.Strategies;

namespace client.Presentation.ViewModels;

public class DashboardViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;

    private ObservableCollection<ProjectTask> _upcomingTasks = [];
    private List<ITask> _tasks = [];
    private ObservableCollection<Notification> _notifications = [];

    private int _backlogCount;
    private int _inProgressCount;
    private int _inReviewCount;
    private int _finishedCount;

    private ITaskSortStrategy _sortingStrategy;

    public ProjectCalendarViewModel CalendarViewModel { get; } = new();
    public ITaskSortStrategy SortingStrategy
    {
        get => _sortingStrategy;
        set => SetProperty(ref _sortingStrategy, value);
    }

    public ObservableCollection<ProjectTask> UpcomingTasks
    {
        get => _upcomingTasks;
        set => SetProperty(ref _upcomingTasks, value);
    }

    public ObservableCollection<Notification> Notifications
    {
        get => _notifications;
        set => SetProperty(ref _notifications, value);
    }

    public int BacklogCount
    {
        get => _backlogCount;
        set => SetProperty(ref _backlogCount, value);
    }

    public int InProgressCount
    {
        get => _inProgressCount;
        set => SetProperty(ref _inProgressCount, value);
    }
    public int InReviewCount
    {
        get => _inReviewCount;
        set => SetProperty(ref _inReviewCount, value);
    }
    public int FinishedCount
    {
        get => _finishedCount;
        set => SetProperty(ref _finishedCount, value);
    }
    public DashboardViewModel(ILogger logger, IProjectService projectService, ITaskService taskService, IUserService userService)
    {
        _logger = logger;
        _projectService = projectService;
        _taskService = taskService;
        _userService = userService;

        _sortingStrategy = new SortByDeadline();

        _ = GetDashboardMetricsAsync();
    }

    public async Task GetDashboardMetricsAsync()
    {
        try
        {
            _logger.Log(LogLevel.INFO, $"Fetching dashboard metrics for user: {_userService.Id}");
            var allTasks = await _taskService.GetTasksAsync(_userService.Id);

            // Counts and upcoming work are both based on the active user's task assignments.
            BacklogCount = allTasks.Count(t => t.Progress == TaskProgress.Backlog);
            InProgressCount = allTasks.Count(t => t.Progress == TaskProgress.InProgress);
            InReviewCount = allTasks.Count(t => t.Progress == TaskProgress.Review);
            FinishedCount = allTasks.Count(t => t.Progress == TaskProgress.Done);

            _tasks = allTasks
                .Where(t => t.Progress != TaskProgress.Done)
                .Cast<ITask>()
                .ToList();

            var deadlineDates = allTasks
                .Where(t => t.Progress != TaskProgress.Done && t.Deadline != default)
                .Select(t => t.Deadline.Date)
                .ToHashSet();

            // The calendar only needs dates, not full task objects, to render deadline indicators.
            CalendarViewModel.UpdateDeadlines(deadlineDates);

            SortTasks();

            _logger.Log(LogLevel.INFO, "Dashboard metrics successfully loaded");

        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed to load dashboard metrics: {ex.Message}");
        }
    }

    public void ChangeSortingStrategy(ITaskSortStrategy newStrategy)
    {
        if (newStrategy == null || SortingStrategy.GetType() == newStrategy.GetType()) return;

        SortingStrategy = newStrategy;
        _logger.Log(LogLevel.INFO, $"Switched dashboard sorting strategy to {newStrategy.GetType().Name}");

        SortTasks();
    }

    private void SortTasks()
    {
        if (_tasks == null || !_tasks.Any()) return;

        // Sorting strategies operate on ITask, then the UI list is rebuilt from the sorted result.
        SortingStrategy.SortTasks(_tasks);
        UpcomingTasks = new ObservableCollection<ProjectTask>(_tasks.Cast<ProjectTask>());
    }

}
