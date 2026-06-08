// =============================================================================
// Author: Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;

namespace client.Presentation.Commands;

public class ArchiveProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<Project?> _getProject;
    private readonly Func<Project, Task> _archiveAsync;

    // Author: Oliver
    public ArchiveProjectCommand(
        ILogger logger,
        Func<Project?> getProject,
        Func<Project, Task> archiveAsync
    )
    {
        _logger = logger;
        _getProject = getProject;
        _archiveAsync = archiveAsync;
    }

    // Author: Oliver
    public async void Execute(object? param)
    {
        if (!CanExecute(param))
        {
            _logger.Log(LogLevel.WARNING, "ArchiveProjectCommand executed without a selected project.");
            return;
        }

        var project = _getProject();
        if (project == null)
        {
            _logger.Log(LogLevel.WARNING, "ArchiveProjectCommand could not resolve a project.");
            return;
        }

        _logger.Log(LogLevel.INFO, $"ArchiveProjectCommand requested for project {project.Id}.");
        await _archiveAsync(project);
    }

    // Author: Oliver
    public void Undo() { }

    // Author: Oliver
    public bool CanExecute(object? param) => _getProject() != null;

    public event EventHandler? CanExecuteChanged;

    // Author: Oliver
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
