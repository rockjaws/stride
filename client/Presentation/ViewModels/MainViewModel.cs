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

    public MainViewModel(ILogger logger, IProjectService projectService, ITaskService taskService)
    {
        _logger = logger;
        DashboardViewModel = new DashboardViewModel();
        ProjectViewModel = new ProjectViewModel(_logger, projectService, taskService);
        _currentView = DashboardViewModel;
        ChangeViewCommand = new ChangeViewCommand(this);
        CreateNewProjectCommand = new CreateNewProjectCommand(
            _logger,
            projectService,
            AddCreatedProject
        );
    }

    public ICommand ChangeViewCommand { get; }

    public CreateNewProjectCommand CreateNewProjectCommand { get; }

    public ProjectViewModel ProjectViewModel { get; }
    public DashboardViewModel DashboardViewModel { get; }

    public object CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    public void SetCurrentView(object viewModel)
    {
        CurrentView = viewModel;
    }

    private void AddCreatedProject(Project project)
    {
        if (project == null)
            return;

        ProjectViewModel.AddCreatedProject(project);
        CurrentView = ProjectViewModel;
    }
}
