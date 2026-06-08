// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using System.Collections.ObjectModel;
using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Commands;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class ArchiveViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly IUserService _userService;
    private Project? _selectedProject;

    public ObservableCollection<Project> ArchivedProjects { get; }
    public RestoreProjectCommand RestoreProjectCommand { get; }
    public DeleteProjectCommand DeleteProjectCommand { get; }

    public Project? SelectedProject
    {
        get => _selectedProject;
        set => SetProperty(ref _selectedProject, value);
    }

    // Author: Nicolaj and Oliver
    public ArchiveViewModel(
        ILogger logger,
        IProjectService projectService,
        IUserService userService
    )
    {
        _logger = logger;
        _projectService = projectService;
        _userService = userService;
        _projectService.ProjectsChanged += (s, e) => _ = GetProjectsAsync();
        ArchivedProjects = [];
        RestoreProjectCommand = new RestoreProjectCommand(
            logger,
            () => SelectedProject,
            UnarchiveProjectAsync
        );
        DeleteProjectCommand = new DeleteProjectCommand(
            _logger,
            () => SelectedProject!,
            DeleteProjectAsync
        );
        _ = GetProjectsAsync();
    }

    // Author: Nicolaj and Oliver
    private async Task UnarchiveProjectAsync(Project project)
    {
        if (project.Id is not int id)
            return;

        project.Archive();
        try
        {
            await _projectService.SetProjectArchivedAsync(id, false);
            _logger.Log(LogLevel.INFO, $"Unarchived Project {id}");
            ArchivedProjects.Remove(project);
        }
        catch (Exception ex)
        {
            project.UnArchive();
            _logger.Log(LogLevel.ERROR, $"Failed To Unarchive Project {id}: {ex.Message}");
        }
    }

    // Author: Oliver
    private async Task DeleteProjectAsync(Project project)
    {
        if (project.Id is not int id)
            return;

        try
        {
            await _projectService.DeleteProjectAsync(id);
            _logger.Log(LogLevel.INFO, $"Deleted Project {id}");
            ArchivedProjects.Remove(project);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Delete Project {id}: {ex.Message}");
        }
    }

    // Author: Oliver
    private async Task GetProjectsAsync()
    {
        try
        {
            // The API returns only projects associated with the active user, including every task in those projects.
            var projects = await _projectService.GetProjectsAsync(_userService.Id);

            ArchivedProjects.Clear();
            foreach (var project in projects)
            {
                if (project.IsArchived)
                {
                    ArchivedProjects.Add(project);
                }
            }
            _logger.Log(LogLevel.INFO, "All Projects Fetched Succesfully.");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Load Projects: {ex.Message}");
        }
    }
}
