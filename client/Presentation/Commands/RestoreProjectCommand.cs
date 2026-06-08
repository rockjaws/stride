// =============================================================================
// Author: Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;

namespace client.Presentation.Commands;

public class RestoreProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<Project?> _getProject;
    private readonly Func<Project, Task> _restoreAsync;

    // Author: Oliver
    public RestoreProjectCommand(
        ILogger logger,
        Func<Project?> getProject,
        Func<Project, Task> restoreAsync
    )
    {
        _logger = logger;
        _getProject = getProject;
        _restoreAsync = restoreAsync;
    }

    // Author: Oliver
    public async void Execute(object? parameter)
    {
        var project = parameter as Project ?? _getProject();

        if (project != null)
        {
            _logger.Log(LogLevel.INFO, $"RestoreProjectCommand requested for project {project.Id}.");
            await _restoreAsync(project);
            return;
        }

        _logger.Log(LogLevel.WARNING, "RestoreProjectCommand executed without a project.");
    }

    // Author: Oliver
    public bool CanExecute(object? parameter)
    {
        return parameter is Project || _getProject() != null;
    }

    // Author: Oliver
    public void Undo() { }

    public event EventHandler? CanExecuteChanged;
}
