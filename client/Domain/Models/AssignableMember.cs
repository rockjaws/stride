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
        _isAssigned = task.UsersAssigned?.Any(x => x.Id == user.Id) == true;
    }
}
