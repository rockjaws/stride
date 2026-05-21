using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IUserService
{
    int Id { get; }
    Task<List<User>> GetUsersAsync();
}
