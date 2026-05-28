namespace client.Application.Interfaces;

public interface IProject
{
    int? Id { get; }
    string Title { get; }
    string Description { get; }
    bool IsArchived { get; }
    DateTime StartDate { get; }
    DateTime Deadline { get; }
    List<IChatChannel> ChatChannels { get; }
    List<ITask> Tasks { get; }
    void Archive();
    void UnArchive();
}
