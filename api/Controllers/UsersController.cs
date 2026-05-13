using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
  private readonly IUserRepository _repository;

  public UsersController(IUserRepository repository)
  {
    _repository = repository;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<User>>> GetUsers()
  {
    var users = await _repository.GetAllUsersAsync();
    return Ok(users);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<User?>> GetUser(int id)
  {
    var user = await _repository.GetUserByIdAsync(id);
    if (user == null)
    {
      return NotFound();
    }
    return Ok(user);
  }

  [HttpPost]
  public async Task<ActionResult> CreateUser(User user)
  {
    await _repository.AddUserAsync(user);
    await _repository.SaveChangesAsync();

    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
  }


  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(int id)
  {
    var project = await _repository.GetUserByIdAsync(id);
    if (project == null)
    {
      return NotFound();
    }

    await _repository.DeleteUserAsync(project);
    await _repository.SaveChangesAsync();

    return NoContent();
  }
}
