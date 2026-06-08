// =============================================================================
// Author: Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class CreateNewProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private readonly Func<Project, Task> _createProjectAsync;

    // Author: Oliver
    public CreateNewProjectCommand(
        ILogger logger,
        IUserService userService,
        Func<Project, Task> createProjectAsync
    )
    {
        _logger = logger;
        _userService = userService;
        _createProjectAsync = createProjectAsync;
    }

    // Author: Oliver
    public async void Execute(object? param)
    {
        if (!CanExecute(param))
        {
            _logger.Log(client.Domain.Enum.LogLevel.WARNING, "CreateNewProjectCommand cannot execute.");
            return;
        }

        var vm = new NewProjectViewModel(_logger, _userService);
        // The command handles dialog lifetime; the injected callback handles persistence.
        var window = new NewProjectWindow { DataContext = vm };

        if (window.ShowDialog() == true)
        {
            try
            {
                Project project = vm.CreateProject();
                await _createProjectAsync(project);
            }
            catch (Exception ex)
            {
                _logger.Log(client.Domain.Enum.LogLevel.ERROR, $"Failed To Create Project: {ex.Message}");
            }
            return;
        }

        _logger.Log(client.Domain.Enum.LogLevel.INFO, "Create project dialog cancelled.");
    }

    // Author: Oliver
    public void Undo() { }

    // Author: Oliver
    public bool CanExecute(object? param) => true;

    public event EventHandler? CanExecuteChanged;

    // Author: Oliver
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
