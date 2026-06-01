using client.Application.Interfaces;
using client.Domain.Models;

namespace client.Presentation.Commands;

public class ArchiveProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<Project?> _getProject;
    private readonly Func<Project, Task> _archiveAsync;
    private readonly Func<Project, Task> _unarchiveAsync;

    public ArchiveProjectCommand(
        ILogger logger,
        Func<Project?> getProject,
        Func<Project, Task> archiveAsync,
        Func<Project, Task> unarchiveAsync
    )
    {
        _logger = logger;
        _getProject = getProject;
        _archiveAsync = archiveAsync;
        _unarchiveAsync = unarchiveAsync;
    }

    public async void Execute(object? param)
    {
        if (!CanExecute(param))
            return;

        var project = _getProject();
        if (project == null)
            return;

        if (project.IsArchived)
            await _unarchiveAsync(project);
        else
            await _archiveAsync(project);
    }

    public void Undo() { }

    public bool CanExecute(object? param) => _getProject() != null;

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
