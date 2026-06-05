using System.Windows.Input;
using System.Windows.Threading;

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    private readonly DispatcherTimer _notificationTimer;
    private object _currentView;
    private string _toastText = string.Empty;
    private bool _isToastVisible;
    private bool _isCheckingNotifications;

    public DashboardViewModel DashboardViewModel { get; }
    public ProjectViewModel ProjectViewModel { get; }
    public TaskViewModel TaskViewModel { get; }
    public ChatViewModel ChatViewModel { get; }
    public ArchiveViewModel ArchiveViewModel { get; }

    public ICommand ChangeViewCommand { get; }
    public ICommand CreateNewProjectCommand { get; }

    public string ToastText
    {
        get => _toastText;
        set => SetProperty(ref _toastText, value);
    }

    public bool IsToastVisible
    {
        get => _isToastVisible;
        set => SetProperty(ref _isToastVisible, value);
    }

    public object CurrentView
    {
        get => _currentView;
        set
        {
            if (SetProperty(ref _currentView, value))
            {
                OnPropertyChanged(nameof(IsDashboardSelected));
                OnPropertyChanged(nameof(IsProjectsSelected));
                OnPropertyChanged(nameof(IsTasksSelected));
                OnPropertyChanged(nameof(IsChatsSelected));
                OnPropertyChanged(nameof(IsArchiveSelected));
            }
        }
    }
    public bool IsDashboardSelected => CurrentView == DashboardViewModel;
    public bool IsProjectsSelected => CurrentView == ProjectViewModel;
    public bool IsTasksSelected => CurrentView == TaskViewModel;
    public bool IsChatsSelected => CurrentView == ChatViewModel;
    public bool IsArchiveSelected => CurrentView == ArchiveViewModel;

    public MainViewModel(
        ILogger logger,
        DashboardViewModel dashboardViewModel,
        ProjectViewModel projectViewModel,
        TaskViewModel taskViewModel,
        ChatViewModel chatViewModel,
        ArchiveViewModel archiveViewModel,
        INotificationService notificationService,
        IUserService userService
    )
    {
        _logger = logger;
        _notificationService = notificationService;
        _userService = userService;

        DashboardViewModel = dashboardViewModel;
        ProjectViewModel = projectViewModel;
        TaskViewModel = taskViewModel;
        ChatViewModel = chatViewModel;
        ArchiveViewModel = archiveViewModel;

        CurrentView = DashboardViewModel;

        ChangeViewCommand = new ChangeViewCommand(_logger, this);
        CreateNewProjectCommand = new CreateNewProjectCommand(_logger, _userService, CreateProjectAsync);

        _notificationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        // DispatcherTimer runs on the UI thread, so toast bindings can be updated directly.
        _notificationTimer.Tick += async (_, _) => await CheckNotificationsAsync();
        _logger.Log(LogLevel.INFO, "MainViewModel initialized.");
    }


    public void StartNotificationPolling()
    {
        if (_notificationTimer.IsEnabled)
        {
            _logger.Log(LogLevel.INFO, "Notification polling already running.");
            return;
        }

        _notificationTimer.Start();
        _logger.Log(LogLevel.INFO, "Notification polling started.");
        // Run once immediately so unread notifications are not delayed until the first timer tick.
        _ = CheckNotificationsAsync();
    }

    public void SetCurrentView(object viewModel)
    {
        CurrentView = viewModel;
        _logger.Log(LogLevel.INFO, $"Current view set to {viewModel.GetType().Name}.");

        // Reload when opening the Tasks tab so edits made elsewhere are picked up.
        if (ReferenceEquals(viewModel, TaskViewModel))
            _ = TaskViewModel.LoadTasksAsync();
    }

    private async Task CreateProjectAsync(Project project)
    {
        _logger.Log(LogLevel.INFO, $"MainViewModel creating project: {project.Title}");
        await ProjectViewModel.CreateProjectAsync(project);
        ChatViewModel.AddProject(ProjectViewModel.ListOfProjects.Last());
        CurrentView = ProjectViewModel;
        _logger.Log(LogLevel.INFO, "Created project added to chat view and project view selected.");
    }

    private async Task CheckNotificationsAsync()
    {
        // DispatcherTimer can tick again while a slow API call is still running.
        if (_isCheckingNotifications)
            return;

        _isCheckingNotifications = true;

        try
        {
            var notifications = await _notificationService.GetNotificationsAsync(_userService.Id);
            var unreadNotification = notifications
                .Where(notification => !notification.IsRead)
                .OrderBy(notification => notification.SentAt)
                .FirstOrDefault();

            if (unreadNotification == null)
                return;

            // Mark after showing so missed toasts remain unread if the display step fails.
            await ShowToastAsync(unreadNotification.Text);
            await _notificationService.MarkAsReadAsync(_userService.Id, unreadNotification.Id);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Check Notifications: {ex.Message}");
        }
        finally
        {
            _isCheckingNotifications = false;
        }
    }

    private async Task ShowToastAsync(string text)
    {
        ToastText = text;
        IsToastVisible = true;
        _logger.Log(LogLevel.INFO, $"Showing notification toast: {text}");

        // Keep the toast simple for now; future queueing can build on this display delay.
        await Task.Delay(TimeSpan.FromSeconds(4));

        IsToastVisible = false;
    }
}
