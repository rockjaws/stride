// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

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

    // Author: Nicolaj and Oliver
    public AssignableMember(User user, ProjectTask task)
    {
        User = user;
        IsAssigned = task.UsersAssigned?.Any(x => x.Id == user.Id) == true;
    }

    // Author: Oliver
    public AssignableMember(User user, bool isAssigned)
    {
        User = user;
        IsAssigned = isAssigned;
    }

    // Author: Oliver
    public AssignableMember(User user, Project project)
    {
        User = user;
        IsAssigned = project.Members.Any(x => x.Id == user.Id);
    }
}
