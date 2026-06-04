using client.Presentation.Common;

namespace client.Domain.Models;

public class AssignableMember : ObservableObject
{
    private bool _isAssigned;
    public User User { get; }
    public bool IsAssigned
    {
        get => _isAssigned;
        set => SetProperty(ref _isAssigned, value);
    }

    public AssignableMember(User user, ProjectTask task)
    {
        User = user;
        IsAssigned = task.UsersAssigned?.Any(x => x.Id == user.Id) == true;
    }

    public AssignableMember(User user, bool isAssigned)
    {
        User = user;
        IsAssigned = isAssigned;
    }

    public AssignableMember(User user, Project project)
    {
        User = user;
        IsAssigned = project.Members.Any(x => x.Id == user.Id);
    }
}
