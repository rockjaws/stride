// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class NewChatViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    // Author: Nicolai
    public NewChatViewModel(ILogger logger)
    {
        _logger = logger;
    }

    // Author: Nicolai and Oliver
    public ChatChannel CreateChannel(int projectId)
    {
        _logger.Log(LogLevel.INFO, $"Prepared New Channel For Project {projectId}: {Name}");
        return new ChatChannel(0, Name, projectId);
    }

}
