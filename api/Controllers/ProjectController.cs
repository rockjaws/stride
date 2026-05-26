using api.DTOs;
using api.Models;
using api.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _repository;
    private readonly IChannelRepository _channelRepository;

    public ProjectController(IProjectRepository repository, IChannelRepository channelRepository)
    {
        _repository = repository;
        _channelRepository = channelRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
    {
        var projects = await _repository.GetAllProjectsAsync();
        var dtos = projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            StartDate = p.StartDate,
            Deadline = p.Deadline,
            ChatChannels = [.. p.ChatChannels.Select(c => new ChannelDto
      {
        Id = c.Id,
        Name = c.Name,
        ProjectId = p.Id
      })],
            Tasks = [.. p.Tasks.Select(t => new ProjectTaskDto
      {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        StartDate = t.StartDate,
        Deadline = t.Deadline,
        Progress = t.Progress,
        Priority = t.Priority,
        ProjectId = p.Id
      })],
            Users = [.. p.Users.Select(u => new UserDto
      {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        WorkMail = u.WorkMail,
        Role = u.Role
      })]
        });
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto?>> GetProject(int id)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        var dto = new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            StartDate = project.StartDate,
            Deadline = project.Deadline,
            ChatChannels = [.. project.ChatChannels.Select(c => new ChannelDto {
          Id = c.Id,
          Name = c.Name,
          ProjectId = project.Id
          })],
            Tasks = [.. project.Tasks.Select(t => new ProjectTaskDto
      {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        StartDate = t.StartDate,
        Deadline = t.Deadline,
        Progress = t.Progress,
        Priority = t.Priority,
        ProjectId = project.Id
      })],
            Users = [.. project.Users.Select(u => new UserDto
      {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        WorkMail = u.WorkMail,
        Role = u.Role
      })]
        };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateProject(ProjectCreateDto dto)
    {
        var project = new Project
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            Deadline = dto.Deadline
        };
        await _repository.AddProjectAsync(project);
        await _repository.SaveChangesAsync();

        var projectDto = new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            StartDate = project.StartDate,
            Deadline = project.Deadline,
            ChatChannels = [],
            Tasks = [],
            Users = []
        };

        return CreatedAtAction(nameof(GetProject), new { id = projectDto.Id }, projectDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProject(int id, ProjectUpdateDto dto)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        project.Title = dto.Title;
        project.Description = dto.Description;
        project.Deadline = dto.Deadline;

        await _repository.UpdateProjectAsync(project);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject(int id)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        await _repository.DeleteProjectAsync(project);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/channels/{channelId}")]
    public async Task<ActionResult<ChannelDto>> GetChannel(int id, int channelId)
    {
        var channel = await _channelRepository.GetChannelByIdAsync(channelId);
        if (channel == null)
        {
            return NotFound();
        }
        return Ok(new ChannelDto { Id = channel.Id, Name = channel.Name, ProjectId = channel.ProjectId });
    }

    [HttpPost("{id}/channels")]
    public async Task<ActionResult> CreateNewChannel(int id, ChannelCreateDto dto)
    {
        var project = await _repository.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        var chatChannel = new ChatChannel
        {
            Name = dto.Name,
            ProjectId = id
        };


        await _channelRepository.CreateChannelAsync(chatChannel);
        await _channelRepository.SaveChangesAsync();

        var chatChannelDto = new ChannelDto
        {
            Id = chatChannel.Id,
            Name = chatChannel.Name,
            ProjectId = chatChannel.ProjectId
        };

        return CreatedAtAction(nameof(GetChannel), new { id, channelId = chatChannel.Id }, chatChannelDto);
    }
}
