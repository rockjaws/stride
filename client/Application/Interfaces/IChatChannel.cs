namespace client.Application.Interfaces;

public interface IChatChannel
{
    int Id { get; }
    List<IMessage> Messages { get; }
    string Name { get; }
}
