using client.Application.Interfaces;

namespace client.Presentation.ViewModels;

public class TaskViewModel
{
    private readonly ILogger _logger;
    private readonly ITaskService _taskService;

    public TaskViewModel(ILogger logger, ITaskService taskService)
    {
        _logger = logger;
        _taskService = taskService;
    }
}
