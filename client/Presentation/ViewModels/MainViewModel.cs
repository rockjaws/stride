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
        set => SetProperty(ref _currentView, value);
    }

    public MainViewModel(
        ILogger logger,
        DashboardViewModel dashboardViewModel,
        ProjectViewModel projectViewModel,
        TaskViewModel taskViewModel,
        ChatViewModel chatViewModel,
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

        TaskViewModel.TaskUpdated += ProjectViewModel.ApplyExternalTaskUpdate;
        TaskViewModel.TaskDeleted += ProjectViewModel.ApplyExternalTaskDelete;

        CurrentView = DashboardViewModel;

        ChangeViewCommand = new ChangeViewCommand(_logger, this);

        CreateNewProjectCommand = new CreateNewProjectCommand(_logger, CreateProjectAsync);

        _notificationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _notificationTimer.Tick += async (_, _) => await CheckNotificationsAsync();
    }

    public void StartNotificationPolling()
    {
        if (_notificationTimer.IsEnabled)
            return;

        _notificationTimer.Start();
        _ = CheckNotificationsAsync();
    }

    public void SetCurrentView(object viewModel)
    {
        CurrentView = viewModel;

        if (ReferenceEquals(viewModel, TaskViewModel))
            _ = TaskViewModel.LoadTasksAsync();
    }

    private async Task CreateProjectAsync(Project project)
    {
        await ProjectViewModel.CreateProjectAsync(project);
        CurrentView = ProjectViewModel;
    }

    private async Task CheckNotificationsAsync()
    {
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

        await Task.Delay(TimeSpan.FromSeconds(4));

        IsToastVisible = false;
    }
}
