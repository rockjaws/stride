namespace client.Application.Interfaces;

public interface IProject
{
    int Id { get; }
    string Title { get; }
    string Description { get; }
    DateTime StartDate { get; }
    DateTime Deadline { get; }
    List<IChatChannel> ChatChannels { get; }
}
