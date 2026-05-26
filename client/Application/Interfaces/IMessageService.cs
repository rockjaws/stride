using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IMessageService
{
    Task<List<Message>> GetMessagesAsync(int id);
    Task<Message> SendMessageAsync(int id, string text, int userId);
}
