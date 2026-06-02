using System.Collections.ObjectModel;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Presentation.Algorithms;
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

        _taskService.TasksChanged += OnGlobalStateChange;
        _projectService.ProjectsChanged += OnGlobalStateChange;

        _ = GetDashboardMetricsAsync();
    }

    private void OnGlobalStateChange(object? sender, EventArgs e)
    {
        _ = GetDashboardMetricsAsync();
    }

    public async Task GetDashboardMetricsAsync()
    {
        try
        {
            _logger.Log(LogLevel.INFO, $"Fetching dashboard metrics for user: {_userService.Id}");

            var currentUserId = _userService.Id;
            var allProjects = await _projectService.GetProjectsAsync(_userService.Id);

            // 1. Reset counters
            int backlogCount = 0;
            int inProgressCount = 0;
            int inReviewCount = 0;
            int finishedCount = 0;

            // 2. Prepare target collections (Pre-allocating objects we actually need)
            var activeTasks = new List<ITask>();
            var deadlineDates = new HashSet<DateTime>();

            // 3. Single-pass iteration across active projects and tasks
            foreach (var project in allProjects)
            {
                if (project.IsArchived) continue; // Skip archived projects efficiently

                foreach (var task in project.Tasks)
                {
                    if (task.UsersAssigned == null || !task.UsersAssigned.Any(u => u.Id == currentUserId))
                    {
                        continue;
                    }
                    // Increment counters based on progress
                    switch (task.Progress)
                    {
                        case TaskProgress.Backlog: backlogCount++; break;
                        case TaskProgress.InProgress: inProgressCount++; break;
                        case TaskProgress.Review: inReviewCount++; break;
                        case TaskProgress.Done: finishedCount++; break;
                    }

                    // Gather incomplete task data
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

            // 4. Assign values to properties all at once
            BacklogCount = backlogCount;
            InProgressCount = inProgressCount;
            InReviewCount = inReviewCount;
            FinishedCount = finishedCount;

            _tasks = activeTasks;

            // Update the UI/ViewModel
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

    public void Dispose()
    {
        _taskService.TasksChanged -= OnGlobalStateChange;
        _projectService.ProjectsChanged -= OnGlobalStateChange;
    }
}
