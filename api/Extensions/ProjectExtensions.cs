using api.DTOs;
using api.Models;

namespace api.Extensions;

public static class ProjectExtensions
{
    // Author: Nicolai and Oliver
    public static ProjectDto ToDto(this Project p)
    {
        return new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            StartDate = p.StartDate,
            Deadline = p.Deadline,
            IsArchived = p.IsArchived,

            ChatChannels =
                    [
                        .. p.ChatChannels.Select(c => new ChannelDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProjectId = p.Id,
                })
                    ],

            Tasks =
                    [
                        .. p.Tasks.Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    Deadline = t.Deadline,
                    Progress = t.Progress,
                    Priority = t.Priority,
                    ProjectId = p.Id,

                    Users =
                    [
                        .. t.Users.Select(u => new UserDto
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            WorkMail = u.WorkMail
                        })
                    ]
                })
                    ],

            Users =
                    [
                        .. p.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    WorkMail = u.WorkMail,
                })
                    ]
        };
    }
}
