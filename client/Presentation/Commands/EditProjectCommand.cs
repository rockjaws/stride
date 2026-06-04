using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class EditProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private readonly Func<Project?> _getProject;
    private readonly Func<Project, Task> _updateProjectAsync;

    public EditProjectCommand(
        ILogger logger,
        IUserService userService,
        Func<Project?> getProject,
        Func<Project, Task> updateProjectAsync
    )
    {
        _logger = logger;
        _userService = userService;
        _getProject = getProject;
        _updateProjectAsync = updateProjectAsync;
    }

    public async void Execute(object? param)
    {
        if (!CanExecute(param))
        {
            _logger.Log(client.Domain.Enum.LogLevel.WARNING, "EditProjectCommand cannot execute.");
            return;
        }

        var project = _getProject();
        if (project == null)
        {
            _logger.Log(client.Domain.Enum.LogLevel.WARNING, "EditProjectCommand could not resolve a project.");
            return;
        }

        var vm = new EditProjectViewModel(_logger, project, _userService);
        var window = new EditProjectWindow { DataContext = vm };

        if (window.ShowDialog() == true)
        {
            try
            {
                Project? updatedProject = vm.UpdateProject();
                if (updatedProject == null)
                    return;

                await _updateProjectAsync(updatedProject);
            }
            catch (Exception ex)
            {
                _logger.Log(
                    client.Domain.Enum.LogLevel.ERROR,
                    $"Failed To Save Project Changes: {ex.Message}"
                );
            }
            return;
        }

        _logger.Log(client.Domain.Enum.LogLevel.INFO, $"Edit project dialog cancelled for project {project.Id}.");
    }

    public void Undo() { }

    public bool CanExecute(object? param) => _getProject() != null;

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
