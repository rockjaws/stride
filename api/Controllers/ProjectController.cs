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
  public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
  {
    var projects = await _repository.GetAllProjectsAsync();
    return Ok(projects);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Project?>> GetProject(int id)
  {
    var project = await _repository.GetProjectByIdAsync(id);
    if (project == null)
    {
      return NotFound();
    }
    return Ok(project);
  }

  [HttpPost]
  public async Task<ActionResult> CreateProject(Project project)
  {
    await _repository.AddProjectAsync(project);
    await _repository.SaveChangesAsync();

    return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
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
