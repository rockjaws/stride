using api.Models;

namespace api.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetMessagesByChannelIdAsync(int id);
    Task CreateMessageAsync(Message message);
    Task SaveChangesAsync();
}
