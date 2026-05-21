using System.Collections.ObjectModel;

using client.Application.Interfaces;
using client.Domain.Models;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class ChatViewModel : ObservableObject
{
    private readonly ILogger _logger;

    // message service

    private ObservableCollection<Message> _messages;
    private ObservableCollection<ChatChannel> _chatChannels;

    public ChatViewModel(ILogger logger)
    {
        _logger = logger;
    }
}
