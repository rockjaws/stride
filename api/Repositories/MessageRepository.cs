using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetMessagesByChannelIdAsync(int id)
    {
        return await _context.Messages
            .AsSplitQuery()
            .Where(m => m.ChannelId == id)
            .Include(m => m.User)
            .OrderBy(m => m.Time)
            .ToListAsync();
    }

    public async Task CreateMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
