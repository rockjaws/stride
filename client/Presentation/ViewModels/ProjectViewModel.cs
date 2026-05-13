using System.Collections.ObjectModel;
using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class ProjectViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private Project _selectedProject;

    public Project SelectedProject
    {
        get => _selectedProject;
        set
        {
            SetProperty(ref _selectedProject, value);
            _logger.Log(LogLevel.INFO, $"New Project Selected: {_selectedProject.Title}");
        }
    }

    public ObservableCollection<Project> ListOfProjects { get; }

    public ProjectViewModel(ILogger logger, IProjectService projectService)
    {
        _logger = logger;
        _projectService = projectService;
        ListOfProjects = new ObservableCollection<Project>();
        _ = GetProjectsAsync();
    }

    public async Task GetProjectsAsync()
    {
        try
        {
            var projects = await _projectService.GetProjectsAsync();

            ListOfProjects.Clear();
            foreach (var project in projects)
            {
                ListOfProjects.Add(project);
            }
            _logger.Log(LogLevel.INFO, "All Projects Fetched Succesfully.");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.ERROR, $"Failed To Load Projects: {ex.Message}");
        }
    }
}
