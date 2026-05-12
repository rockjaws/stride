namespace client.Application.Interfaces;

public interface IMessage
{
    int Id { get; }
    string Text { get; }
    DateTime Time { get; }
}
