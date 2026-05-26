using client.Application.Interfaces;

namespace client.Domain.Models;

public class Message : IMessage
{
    public int Id { get; }
    public string Text { get; }
    public DateTime Time { get; }
    public int ChannelId { get; }
    public int UserId { get; }
    public string SenderName { get; } = "Unknown";

    public Message(int id, string text, DateTime time, int channelId, int userId, string senderName)
    {
        Id = id;
        Text = text;
        Time = time;
        ChannelId = channelId;
        UserId = userId;
        SenderName = senderName;
    }
}
