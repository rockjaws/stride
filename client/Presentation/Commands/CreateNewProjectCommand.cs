using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.ViewModels;
using client.Presentation.Views;

namespace client.Presentation.Commands;

public class CreateNewProjectCommand : IUndoableCommand
{
    private readonly ILogger _logger;
    private readonly Func<Project, Task> _createProjectAsync;

    public CreateNewProjectCommand(
        ILogger logger,
        Func<Project, Task> createProjectAsync
    )
    {
        _logger = logger;
        _createProjectAsync = createProjectAsync;
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
                await _createProjectAsync(project);
            }
            catch (Exception ex)
            {
                _logger.Log(client.Domain.Enum.LogLevel.ERROR, $"Failed To Create Project: {ex.Message}");
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
