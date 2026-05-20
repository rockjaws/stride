using client.Application.Interfaces;

namespace client.Presentation.ViewModels;

public class DashboardViewModel
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;

    public DashboardViewModel(ILogger logger, IProjectService projectService, ITaskService taskService)
    {
        _logger = logger;
        _projectService = projectService;
        _taskService = taskService;
    }
}
