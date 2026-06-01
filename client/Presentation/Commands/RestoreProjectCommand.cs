using client.Application.Interfaces;
using client.Domain.Models;

namespace client.Presentation.Commands;

public class RestoreProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<Project?> _getProject;
    private readonly Func<Project, Task> _restoreAsync;

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

    public async void Execute(object? parameter)
    {
        var project = parameter as Project ?? _getProject();

        if (project != null)
            await _restoreAsync(project);
    }

    public bool CanExecute(object? parameter)
    {
        return parameter is Project || _getProject() != null;
    }

    public void Undo() { }

    public event EventHandler? CanExecuteChanged;
}
