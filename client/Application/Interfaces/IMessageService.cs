// =============================================================================
// Author: Nicolai
// =============================================================================

using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IMessageService
{
    // Author: Nicolai
    Task<List<Message>> GetMessagesAsync(int id);
    // Author: Nicolai
    Task<Message> SendMessageAsync(int id, string text, int userId);
}
