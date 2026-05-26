using System.Windows;
using client.Application.Interfaces;
using client.Application.Services;
using client.Domain.Enum;
using client.Infrastructure.Logging;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Services
        ILogger logger = new Logger();
        IProjectService projectService = new ProjectService();
        ITaskService taskService = new TaskService();
        INotificationService notificationService = new NotificationService();
        // Temporary active user selection until proper login/session handling exists.
        IUserService userService = new UserService(1);
        logger.Log(LogLevel.INFO, "Application Starting..");

        // Child viewmodels
        var dashboardViewModel = new DashboardViewModel(
            logger,
            projectService,
            taskService,
            userService
        );

        var projectViewModel = new ProjectViewModel(
            logger,
            projectService,
            taskService,
            userService
        );

        var taskViewModel = new TaskViewModel(logger, taskService, userService);

        var chatViewModel = new ChatViewModel(logger);

        // Main viewmodel
        var viewModel = new MainViewModel(
            logger,
            dashboardViewModel,
            projectViewModel,
            taskViewModel,
            chatViewModel,
            notificationService,
            userService
        );

        // Main window
        var mainWindow = new MainWindow(viewModel);

        MainWindow = mainWindow;
        mainWindow.Show();
        viewModel.StartNotificationPolling();
    }
}
