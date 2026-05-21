using client.Application.Interfaces;

namespace client.Presentation.ViewModels;

public class DashboardViewModel
{
    private readonly ILogger _logger;
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;

    public DashboardViewModel(
        ILogger logger,
        IProjectService projectService,
        ITaskService taskService,
        IUserService userService
    )
    {
        _logger = logger;
        _projectService = projectService;
        _taskService = taskService;
        _userService = userService;
    }
}
