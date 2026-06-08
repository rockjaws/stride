using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    // Author: Nicolai and Oliver
    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    // Author: Nicolai and Oliver
    public async Task<IEnumerable<Message>> GetMessagesByChannelIdAsync(int id)
    {
        return await _context.Messages
            .AsSplitQuery()
            .Where(m => m.ChannelId == id)
            .Include(m => m.User)
            .OrderBy(m => m.Time)
            .ToListAsync();
    }

    // Author: Nicolai and Oliver
    public async Task CreateMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
    }

    // Author: Nicolai and Oliver
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
