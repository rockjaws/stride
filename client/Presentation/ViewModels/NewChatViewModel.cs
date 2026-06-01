using client.Application.Interfaces;
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

    public NewChatViewModel(ILogger logger)
    {
        _logger = logger;
    }

    public ChatChannel CreateChannel(int projectId)
    {
        return new ChatChannel(0, Name, projectId);
    }

}
