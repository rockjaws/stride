// =============================================================================
// Author: Oliver
// =============================================================================

using System.Net.Http;
using System.Net.Http.Json;

using client.Application.Interfaces;
using client.Domain.Models;

namespace client.Application.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    public int Id { get; }

    // Author: Oliver
    public UserService(int id, HttpClient httpClient)
    {
        Id = id;
        _httpClient = httpClient;
    }

    // Author: Oliver
    public async Task<List<User>> GetUsersAsync()
    {
        var userDtos = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/users") ?? [];
        return [.. userDtos.Select(ToUser)];
    }

    // Author: Oliver
    public async Task<List<User>> GetUsersAsync(int projectId)
    {
        var userDtos =
            await _httpClient.GetFromJsonAsync<List<UserDto>>($"api/users?projectId={projectId}")
            ?? [];
        return [.. userDtos.Select(ToUser)];
    }

    // Author: Oliver
    private static User ToUser(UserDto dto)
    {
        return new User(dto.Id, dto.FirstName, dto.LastName, dto.WorkMail);
    }

    private sealed class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string WorkMail { get; set; } = string.Empty;
    }
}
