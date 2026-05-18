using api.Models;

namespace api.Repositories;

public interface IChannelRepository
{
  Task<ChatChannel?> GetChannelByIdAsync(int id);
  Task CreateChannelAsync(ChatChannel chatChannel);
  Task DeleteChannelAsync(ChatChannel chatChannel);
  Task SaveChangesAsync();
}
