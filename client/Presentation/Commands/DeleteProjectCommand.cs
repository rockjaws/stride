// =============================================================================
// Author: Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;

namespace client.Presentation.Commands;

public class DeleteProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<Project> _getProject;
    private readonly Func<Project, Task> _deleteProject;

    // Author: Oliver
    public DeleteProjectCommand(
        ILogger logger,
        Func<Project> getProject,
        Func<Project, Task> deleteProject
    )
    {
        _logger = logger;
        _deleteProject = deleteProject;
        _getProject = getProject;
    }

    // Author: Oliver
    public async void Execute(object? parameter)
    {
        var project = parameter as Project ?? _getProject?.Invoke();

        if (project != null)
        {
            try
            {
                _logger.Log(LogLevel.INFO, $"DeleteProjectCommand requested for project {project.Id}.");
                await _deleteProject(project);
            }
            catch (Exception ex)
            {
                _logger.Log(
                    LogLevel.ERROR,
                    $"Failed to execute DeleteProjectCommand: {ex.Message}"
                );
            }
            return;
        }

        _logger.Log(LogLevel.WARNING, "DeleteProjectCommand executed without a project.");
    }

    // Author: Oliver
    public bool CanExecute(object? parameter) => _getProject != null;

    // Author: Oliver
    public void Undo() { }

    public event EventHandler? CanExecuteChanged;

    // Author: Oliver
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
