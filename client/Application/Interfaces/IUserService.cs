using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IUserService
{
    int Id { get; }
    Task<List<User>> GetUsersAsync();
    Task<List<User>> GetUsersAsync(int projectId);
}
