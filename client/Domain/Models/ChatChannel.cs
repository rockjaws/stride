using client.Application.Interfaces;

namespace client.Domain.Models;

public class ChatChannel : IChatChannel
{
    public int Id { get; }
    public List<IMessage> Messages { get; private set; }

    public ChatChannel(int id, List<IMessage> messages)
    {
        Id = id;
        Messages = messages;
    }
}
