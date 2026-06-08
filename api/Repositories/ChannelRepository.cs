using api.Data;
using api.Models;

namespace api.Repositories;

public class ChannelRepository : IChannelRepository
{
  private readonly AppDbContext _context;

  public ChannelRepository(AppDbContext context)
  {
    _context = context;
  }


  public async Task<ChatChannel?> GetChannelByIdAsync(int id)
  {
    return await _context.ChatChannels.FindAsync(id);
  }
  public async Task CreateChannelAsync(ChatChannel chatChannel)
  {
    await _context.ChatChannels.AddAsync(chatChannel);
  }
  public async Task DeleteChannelAsync(ChatChannel chatChannel)
  {
    _context.ChatChannels.Remove(chatChannel);
    await Task.CompletedTask;
  }
  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}
