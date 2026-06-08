// =============================================================================
// Author: Oliver
// =============================================================================

using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IUserService
{
    int Id { get; }
    // Author: Oliver
    Task<List<User>> GetUsersAsync();
    // Author: Oliver
    Task<List<User>> GetUsersAsync(int projectId);
}
