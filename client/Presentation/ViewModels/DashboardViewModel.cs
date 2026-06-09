// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using System.Collections.ObjectModel;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;
using client.Presentation.Strategies;

namespace client.Presentation.ViewModels;

public class DashboardViewModel : ObservableObject, IDisposable
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;

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
    // Author: Nicolaj and Oliver
    public DashboardViewModel(ILogger logger, IProjectService projectService, ITaskService taskService, IUserService userService, INotificationService notificationService)
    {
        _logger = logger;
        _projectService = projectService;
        _taskService = taskService;
        _userService = userService;
        _notificationService = notificationService;

        _sortingStrategy = new SortByDeadline();

        _projectService.ProjectsChanged += OnGlobalStateChange;
        _taskService.TasksChanged += OnGlobalStateChange;
        _notificationService.NotificationsChanged += OnGlobalStateChange;

        _ = GetDashboardMetricsAsync();
    }


    // Author: Nicolaj and Oliver
    public async Task GetDashboardMetricsAsync()
    {
        try
        {
            _logger.Log(LogLevel.INFO, $"Fetching dashboard metrics for user: {_userService.Id}");

            var currentUserId = _userService.Id;

            var feedItems = await _notificationService.GetDashboardFeedAsync(currentUserId);
            Notifications.Clear();
            foreach (var feedItem in feedItems)
            {
                Notifications.Add(feedItem);
            }

            var allProjects = await _projectService.GetProjectsAsync(_userService.Id);

            int backlogCount = 0;
            int inProgressCount = 0;
            int inReviewCount = 0;
            int finishedCount = 0;

            var activeTasks = new List<ITask>();
            var deadlineDates = new HashSet<DateTime>();

            // Build counts, the upcoming list, and calendar markers in one pass over assigned tasks.
            foreach (var project in allProjects)
            {
                if (project.IsArchived) continue;

                foreach (var task in project.Tasks)
                {
                    if (task.UsersAssigned == null || !task.UsersAssigned.Any(u => u.Id == currentUserId))
                    {
                        continue;
                    }
                    switch (task.Progress)
                    {
                        case TaskProgress.Backlog: backlogCount++; break;
                        case TaskProgress.InProgress: inProgressCount++; break;
                        case TaskProgress.Review: inReviewCount++; break;
                        case TaskProgress.Done: finishedCount++; break;
                    }

                    if (task.Progress != TaskProgress.Done)
                    {
                        activeTasks.Add(task);

                        if (task.Deadline != default)
                        {
                            deadlineDates.Add(task.Deadline.Date);
                        }
                    }
                }
            }

            // Publish the completed snapshot together to avoid showing partially updated metrics.
            BacklogCount = backlogCount;
            InProgressCount = inProgressCount;
            InReviewCount = inReviewCount;
            FinishedCount = finishedCount;

            _tasks = activeTasks;

            CalendarViewModel.UpdateDeadlines(deadlineDates);
            SortTasks();

            _logger.Log(LogLevel.INFO, "Dashboard metrics successfully loaded");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed to load dashboard metrics: {ex.Message}");
        }
    }

    // Author: Nicolaj
    public void ChangeSortingStrategy(ITaskSortStrategy newStrategy)
    {
        if (newStrategy == null || SortingStrategy.GetType() == newStrategy.GetType()) return;

        SortingStrategy = newStrategy;
        _logger.Log(LogLevel.INFO, $"Switched dashboard sorting strategy to {newStrategy.GetType().Name}");

        SortTasks();
    }

    // Author: Nicolaj and Oliver
    private void SortTasks()
    {
        if (_tasks == null || _tasks.Count == 0) return;

        // Sorting strategies operate on ITask, then the UI list is rebuilt from the sorted result.
        SortingStrategy.SortTasks(_tasks);
        UpcomingTasks = new ObservableCollection<ProjectTask>(_tasks.Cast<ProjectTask>());
    }

    // Author: Nicolaj
    private void OnGlobalStateChange(object? sender, EventArgs e)
    {
        _ = GetDashboardMetricsAsync();
    }

    // Author: Nicolaj
    public void Dispose()
    {
        _taskService.TasksChanged -= OnGlobalStateChange;
        _projectService.ProjectsChanged -= OnGlobalStateChange;
        _notificationService.NotificationsChanged -= OnGlobalStateChange;
    }
}
