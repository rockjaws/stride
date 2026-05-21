using System.Windows.Input;
using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private object _currentView;

    public DashboardViewModel DashboardViewModel { get; }
    public ProjectViewModel ProjectViewModel { get; }
    public TaskViewModel TaskViewModel { get; }
    public ChatViewModel ChatViewModel { get; }

    public ICommand ChangeViewCommand { get; }
    public ICommand CreateNewProjectCommand { get; }

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
        ChatViewModel chatViewModel
    )
    {
        _logger = logger;

        DashboardViewModel = dashboardViewModel;
        ProjectViewModel = projectViewModel;
        TaskViewModel = taskViewModel;
        ChatViewModel = chatViewModel;

        TaskViewModel.TaskUpdated += ProjectViewModel.ApplyExternalTaskUpdate;
        TaskViewModel.TaskDeleted += ProjectViewModel.ApplyExternalTaskDelete;

        CurrentView = DashboardViewModel;

        ChangeViewCommand = new ChangeViewCommand(_logger, this);

        CreateNewProjectCommand = new CreateNewProjectCommand(_logger, CreateProjectAsync);
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
}
