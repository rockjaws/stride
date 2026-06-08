// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Domain.Enum;
using client.Domain.Models;

namespace client.Application.Interfaces;

public interface ITask
{
    int? Id { get; }
    string Title { get; }
    string Description { get; }
    DateTime StartDate { get; }
    DateTime Deadline { get; }
    TaskProgress Progress { get; }
    TaskPriority Priority { get; }
    int? ProjectId { get; }
    List<User>? UsersAssigned { get; set; }
}
