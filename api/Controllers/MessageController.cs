using api.DTOs;
using api.Models;
using api.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/channels/{channelId}/messages")]
public class MessageController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;
    private readonly IChannelRepository _channelRepository;
    private readonly IUserRepository _userRepository;

    public MessageController(IMessageRepository messageRepo, IChannelRepository channelRepo, IUserRepository userRepo)
    {
        _messageRepository = messageRepo;
        _channelRepository = channelRepo;
        _userRepository = userRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(int channelId)
    {
        var channel = await _channelRepository.GetChannelByIdAsync(channelId);
        if (channel == null)
        {
            return NotFound("Channel not found");
        }

        var messages = await _messageRepository.GetMessagesByChannelIdAsync(channelId);
        var dtos = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            Text = m.Text,
            Time = m.Time,
            ChannelId = m.ChannelId,
            SenderUserId = m.User.Id,
            SenderUsername = m.User != null ? $"{m.User.FirstName} {m.User.LastName}" : "Unknown user"
        });

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult> SendMessage(int channelId, MessageCreateDto dto)
    {
        var channel = await _channelRepository.GetChannelByIdAsync(channelId);
        if (channel == null)
        {
            return NotFound("Channel not found");
        }

        var message = new Message
        {
            Text = dto.Text,
            Time = DateTime.Now,
            ChannelId = channelId,
            UserId = dto.UserId
        };

        await _messageRepository.CreateMessageAsync(message);
        await _messageRepository.SaveChangesAsync();

        var user = await _userRepository.GetUserByIdAsync(dto.UserId);
        var resultDto = new MessageDto
        {
            Id = message.Id,
            Text = message.Text,
            Time = message.Time,
            ChannelId = message.ChannelId,
            SenderUserId = message.UserId,
            SenderUsername = message.User != null ? $"{message.User.FirstName} {message.User.LastName}" : "Unknown user"
        };

        return CreatedAtAction(nameof(GetMessages), new { channelId }, resultDto);
    }
}
