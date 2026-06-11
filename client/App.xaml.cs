// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using System.IO;
using System.Net.Http;
using System.Text.Json;
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
    private ILogger? _logger;

    // Author: Nicolaj and Oliver
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        string serverUrl = "http://localhost:5189";
        string apiKey = "DEVELOPMENT_LOCAL_KEY";
        int currentUserId = 1;

        // Deployed builds can override the local defaults without recompiling the client.
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        if (File.Exists(configPath))
        {
            try
            {
                string jsonString = File.ReadAllText(configPath);
                using JsonDocument doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;
                if (root.TryGetProperty("ServerUrl", out var urlProp)) serverUrl = urlProp.GetString();
                if (root.TryGetProperty("ApiKey", out var keyProp)) apiKey = keyProp.GetString();
                if (root.TryGetProperty("CurrentUserId", out JsonElement userElement)) currentUserId = userElement.GetInt32();
            }
            catch
            {
                // Ignore optional config errors and continue with the available/default values.
            }
        }

        var httpClient = new HttpClient { BaseAddress = new Uri(serverUrl) };
        httpClient.DefaultRequestHeaders.Add("X-Stride-API-Key", apiKey);

        // Services
        ILogger logger = new Logger();
        _logger = logger;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        IProjectService projectService = new ProjectService(httpClient);
        TaskService taskService = new TaskService(httpClient);
        INotificationService notificationService = new NotificationService(httpClient);
        IMessageService messageService = new MessageService(httpClient);
        // Temporary active user selection until proper login/session handling exists.
        IUserService userService = new UserService(currentUserId, httpClient);
        logger.Log(LogLevel.INFO, "Application Starting..");

        // Child viewmodels
        var dashboardViewModel = new DashboardViewModel(
            logger,
            projectService,
            taskService,
            userService,
            notificationService
        );

        var projectViewModel = new ProjectViewModel(
            logger,
            projectService,
            taskService,
            userService,
            notificationService
        );

        var taskViewModel = new TaskViewModel(logger, taskService, userService);

        var chatViewModel = new ChatViewModel(logger, projectService, messageService, userService);

        var archiveViewModel = new ArchiveViewModel(
            logger,
            projectService,
            userService,
            notificationService
        );

        // Main viewmodel
        var viewModel = new MainViewModel(
            logger,
            dashboardViewModel,
            projectViewModel,
            taskViewModel,
            chatViewModel,
            archiveViewModel,
            notificationService,
            userService
        );

        // Main window
        var mainWindow = new MainWindow(viewModel);

        MainWindow = mainWindow;
        mainWindow.Show();
        logger.Log(LogLevel.INFO, "Main window shown.");
        viewModel.StartNotificationPolling();
    }

    // Author: Oliver
    protected override void OnExit(ExitEventArgs e)
    {
        _logger?.Log(LogLevel.INFO, $"Application exiting with code {e.ApplicationExitCode}.");
        base.OnExit(e);
    }

    // Author: Oliver
    private void OnDispatcherUnhandledException(
        object sender,
        System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e
    )
    {
        _logger?.Log(LogLevel.ERROR, $"Unhandled UI exception: {e.Exception}");
    }

    // Author: Oliver
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        _logger?.Log(LogLevel.ERROR, $"Unhandled app-domain exception: {e.ExceptionObject}");
    }

    // Author: Oliver
    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _logger?.Log(LogLevel.ERROR, $"Unobserved task exception: {e.Exception}");
    }
}
