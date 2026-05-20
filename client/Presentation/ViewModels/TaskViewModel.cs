using client.Application.Interfaces;

namespace client.Presentation.ViewModels;

public class TaskViewModel
{
    private readonly ILogger _logger;

    public TaskViewModel(ILogger logger)
    {
        _logger = logger;
    }
}
