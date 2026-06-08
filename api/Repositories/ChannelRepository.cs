using api.Data;
using api.Models;

namespace api.Repositories;

public class ChannelRepository : IChannelRepository
{
  private readonly AppDbContext _context;

  // Author: Nicolai and Oliver
  public ChannelRepository(AppDbContext context)
  {
    _context = context;
  }


  // Author: Nicolai and Oliver
  public async Task<ChatChannel?> GetChannelByIdAsync(int id)
  {
    return await _context.ChatChannels.FindAsync(id);
  }
  // Author: Nicolai and Oliver
  public async Task CreateChannelAsync(ChatChannel chatChannel)
  {
    await _context.ChatChannels.AddAsync(chatChannel);
  }
  // Author: Nicolai and Oliver
  public async Task DeleteChannelAsync(ChatChannel chatChannel)
  {
    _context.ChatChannels.Remove(chatChannel);
    await Task.CompletedTask;
  }
  // Author: Nicolai and Oliver
  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}
