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

  public ProjectController(IProjectRepository repository)
  {
    _repository = repository;
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
      ChatChannels = p.ChatChannels,
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
      ChatChannels = project.ChatChannels,
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
      ChatChannels = project.ChatChannels,
      Tasks = [],
      Users = []
    };

    return CreatedAtAction(nameof(GetProject), new { id = projectDto.Id }, projectDto);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateProject(int id, ProjectUpdateDto dto)
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
  public async Task<IActionResult> DeleteProject(int id)
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
}
