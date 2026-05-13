using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/tasks")]
public class ProjectTasksController : ControllerBase
{
  private readonly ITaskRepository _repository;

  public ProjectTasksController(ITaskRepository repository)
  {
    _repository = repository;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<ProjectTask>>> GetProjectTasks()
  {
    var projectTasks = await _repository.GetAllTasksAsync();
    return Ok(projectTasks);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Task?>> GetProjectTask(int id)
  {
    var projectTask = await _repository.GetTaskByIdAsync(id);
    if (projectTask == null)
    {
      return NotFound();
    }
    return Ok(projectTask);
  }

  [HttpPost]
  public async Task<ActionResult> CreateTask(ProjectTask projectTask)
  {
    await _repository.AddTaskAsync(projectTask);
    await _repository.SaveChangesAsync();

    return CreatedAtAction(nameof(GetProjectTask), new { id = projectTask.Id }, projectTask);
  }


  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTask(int id)
  {
    var project = await _repository.GetTaskByIdAsync(id);
    if (project == null)
    {
      return NotFound();
    }

    await _repository.DeleteTaskAsync(project);
    await _repository.SaveChangesAsync();

    return NoContent();
  }
}
