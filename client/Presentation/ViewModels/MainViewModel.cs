using System.Windows.Input;
using client.Application.Interfaces;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private object _currentView;

    public MainViewModel(ILogger logger, IProjectService projectService)
    {
        _logger = logger;
        DashboardViewModel = new DashboardViewModel();
        ProjectViewModel = new ProjectViewModel(_logger, projectService);
        _currentView = DashboardViewModel;
        ChangeViewCommand = new ChangeViewCommand(this);
    }

    public ICommand ChangeViewCommand { get; }

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
}
