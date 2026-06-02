using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;

namespace client.Presentation.Commands;

public class DeleteProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<Project> _getProject;
    private readonly Func<Project, Task> _deleteProject;

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

    public async void Execute(object? parameter)
    {
        var project = parameter as Project ?? _getProject?.Invoke();

        if (project != null)
        {
            try
            {
                await _deleteProject(project);
            }
            catch (Exception ex)
            {
                _logger.Log(
                    LogLevel.ERROR,
                    $"Failed to execute DeleteChannelCommand: {ex.Message}"
                );
            }
        }
    }

    public bool CanExecute(object? parameter) => true;

    public void Undo() { }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
