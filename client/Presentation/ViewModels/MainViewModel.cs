using System.Windows.Input;

using client.Application.Interfaces;
using client.Domain.Enum;
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
        ProjectViewModel projectViewModel)
    {
        _logger = logger;

        DashboardViewModel = dashboardViewModel;
        ProjectViewModel = projectViewModel;

        CurrentView = DashboardViewModel;

        ChangeViewCommand = new ChangeViewCommand(
            _logger,
            this);

        CreateNewProjectCommand = new CreateNewProjectCommand(
            _logger,
            CreateProjectAsync);
    }

    public void SetCurrentView(object viewModel)
    {
        CurrentView = viewModel;
    }

    private async Task CreateProjectAsync(Project project)
    {
        await ProjectViewModel.CreateProjectAsync(project);
        CurrentView = ProjectViewModel;
    }
}
