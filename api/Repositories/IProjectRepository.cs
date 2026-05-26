using api.Models;

namespace api.Repositories;

public interface IProjectRepository
{
<<<<<<< HEAD
  Task<IEnumerable<Project>> GetAllProjectsAsync();
  Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId);
  Task<Project?> GetProjectByIdAsync(int id);
  Task AddProjectAsync(Project project);
  Task UpdateProjectAsync(Project project);
  Task DeleteProjectAsync(Project project);
  Task SaveChangesAsync();
=======
    Task<IEnumerable<Project>> GetAllProjectsAsync();
    Task<Project?> GetProjectByIdAsync(int id);
    Task AddProjectAsync(Project project);
    Task UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(Project project);
    Task SaveChangesAsync();
>>>>>>> 51c8af05bb2ee9efc82699f35cc1e81bca8b3b9c
}
