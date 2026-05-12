using client.Application.Interfaces;

namespace client.Domain.Models;

public class Message : IMessage
{
    public int Id { get; }
    public string Text { get; }
    public DateTime Time { get; }

    public Message(int id, string text, DateTime time)
    {
        Id = id;
        Text = text;
        Time = time;
    }
}
