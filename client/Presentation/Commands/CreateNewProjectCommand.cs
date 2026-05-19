using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class CreateNewProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly Action<Project> _onProjectCreated;

    public CreateNewProjectCommand(
        ILogger logger,
        IProjectService projectService,
        Action<Project> onProjectCreated
    )
    {
        _logger = logger;
        _projectService = projectService;
        _onProjectCreated = onProjectCreated;
    }

    public async void Execute(object? param)
    {
        if (!CanExecute(param))
            return;

        var vm = new NewProjectViewModel(_logger);
        var window = new NewProjectWindow { DataContext = vm };

        if (window.ShowDialog() == true)
        {
            try
            {
                Project project = vm.CreateProject();
                Project savedProject = await _projectService.CreateProjectAsync(project);
                _onProjectCreated(savedProject);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.ERROR, $"Failed To Create Project: {ex.Message}");
            }
        }
    }

    public void Undo() { }

    public bool CanExecute(object? param) => true;

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
