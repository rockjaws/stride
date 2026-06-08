// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Application.Interfaces;

namespace client.Domain.Models;

public class ChatChannel : IChatChannel
{
    public int Id { get; }
    public string Name { get; } = string.Empty;
    public int ProjectId { get; }
    public List<IMessage> Messages { get; private set; }

    // Author: Nicolaj and Oliver
    public ChatChannel(int id, string name, int projectId)
    {
        Id = id;
        Name = name;
        ProjectId = projectId;
    }
}
