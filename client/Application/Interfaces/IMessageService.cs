// =============================================================================
// Author: Nicolaj
// =============================================================================

using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IMessageService
{
    // Author: Nicolaj
    Task<List<Message>> GetMessagesAsync(int id);
    // Author: Nicolaj
    Task<Message> SendMessageAsync(int id, string text, int userId);
}
