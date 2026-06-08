using api.DTOs;
using api.Models;

namespace api.Extensions;

public static class ProjectTaskExtensions
{
    public static ProjectTaskDto ToDto(this ProjectTask t)
    {
        return new ProjectTaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            StartDate = t.StartDate,
            Deadline = t.Deadline,
            Progress = t.Progress,
            Priority = t.Priority,
            ProjectId = t.ProjectId,
            Users = [.. t.Users.Select(u => new UserDto {
           Id = u.Id,
           FirstName = u.FirstName,
           LastName = u.LastName,
           WorkMail = u.WorkMail,
        })],
        };
    }
}
