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

        ILogger logger = new Logger();
        IProjectService projectService = new ProjectService();
        ITaskService taskService = new TaskService();
        logger.Log(LogLevel.INFO, "Application Starting..");

        var viewModel = new MainViewModel(logger, projectService, taskService);
        var mainWindow = new MainWindow(viewModel);

        MainWindow = mainWindow;
        mainWindow.Show();
    }
}
