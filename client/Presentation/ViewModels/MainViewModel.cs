using System.Windows.Input;
using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private object _currentView;

    public MainViewModel(ILogger logger, IProjectService projectService, ITaskService taskService)
    {
        _logger = logger;
        _projectService = projectService;
        DashboardViewModel = new DashboardViewModel();
        ProjectViewModel = new ProjectViewModel(_logger, projectService, taskService);
        _currentView = DashboardViewModel;
        ChangeViewCommand = new ChangeViewCommand(_logger, this);
        CreateNewProjectCommand = new CreateNewProjectCommand(
            _logger,
            CreateProjectAsync
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

    private async Task CreateProjectAsync(Project project)
    {
        try
        {
            Project savedProject = await _projectService.CreateProjectAsync(project);
            AddCreatedProject(savedProject);
            _logger.Log(Domain.Enum.LogLevel.INFO, $"Created Project {savedProject.Id}: {savedProject.Title}");
        }
        catch (Exception ex)
        {
            _logger.Log(Domain.Enum.LogLevel.ERROR, $"Failed To Create Project: {ex.Message}");
        }
    }

    private void AddCreatedProject(Project project)
    {
        if (project == null)
            return;

        ProjectViewModel.AddCreatedProject(project);
        CurrentView = ProjectViewModel;
        _logger.Log(Domain.Enum.LogLevel.INFO, $"Added Created Project To UI: {project.Id}");
    }
}
