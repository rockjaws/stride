using System.Collections.ObjectModel;
using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class ArchiveViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly IUserService _userService;

    public ObservableCollection<Project> ArchivedProjects { get; }

    public ArchiveViewModel(
        ILogger logger,
        IProjectService projectService,
        IUserService userService
    )
    {
        _logger = logger;
        _projectService = projectService;
        _userService = userService;
        ArchivedProjects = [];
        _ = GetProjectsAsync();
    }

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
