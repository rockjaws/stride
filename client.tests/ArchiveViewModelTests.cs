using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;
using client.Presentation.ViewModels;

namespace client.tests;

public class ArchiveViewModelTests
{
    [Fact]
    public async Task RemoteNotification_ReloadsArchivedProjects()
    {
        var projectService = new FakeProjectService();
        var notificationService = new FakeNotificationService();
        var project = new Project(
            7,
            "Shared project",
            string.Empty,
            DateTime.Today,
            DateTime.Today.AddDays(7),
            []
        );
        projectService.Projects = [project];

        using var viewModel = new ArchiveViewModel(
            new NullLogger(),
            projectService,
            new FakeUserService(),
            notificationService
        );

        Assert.Empty(viewModel.ArchivedProjects);

        project.Archive();
        notificationService.RaiseNotificationsChanged();

        await WaitUntilAsync(() => viewModel.ArchivedProjects.Count == 1);

        Assert.Equal(project.Id, viewModel.ArchivedProjects[0].Id);
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            if (condition())
                return;

            await Task.Delay(10);
        }

        Assert.True(condition(), "The archive view did not reload after the remote notification.");
    }

    private sealed class FakeProjectService : IProjectService
    {
        public List<Project> Projects { get; set; } = [];

        public event EventHandler? ProjectsChanged;

        public Task<List<Project>> GetProjectsAsync() => Task.FromResult(Projects);

        public Task<List<Project>> GetProjectsAsync(int userId) => Task.FromResult(Projects);

        public Task<Project> CreateProjectAsync(Project project, int userId) =>
            Task.FromResult(project);

        public Task<Project> UpdateProjectAsync(Project project) => Task.FromResult(project);

        public Task<ChatChannel> CreateChannelAsync(int projectId, string name) =>
            Task.FromResult(new ChatChannel(1, name, projectId));

        public Task DeleteProjectAsync(int id) => Task.CompletedTask;

        public Task DeleteChannelAsync(int id, int projectId) => Task.CompletedTask;

        public Task SetProjectArchivedAsync(int id, bool isArchived) => Task.CompletedTask;
    }

    private sealed class FakeNotificationService : INotificationService
    {
        public event EventHandler? NotificationsChanged;

        public void RaiseNotificationsChanged()
        {
            NotificationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public Task<List<Notification>> GetNotificationsAsync(int userId) =>
            Task.FromResult(new List<Notification>());

        public Task MarkAsReadAsync(int userId, int notificationId) => Task.CompletedTask;

        public Task<List<Notification>> GetDashboardFeedAsync(int userId) =>
            Task.FromResult(new List<Notification>());

        public Task<List<Notification>> GetProjectFeedAsync(int projectId, int userId) =>
            Task.FromResult(new List<Notification>());
    }

    private sealed class FakeUserService : IUserService
    {
        public int Id => 1;

        public Task<List<User>> GetUsersAsync() => Task.FromResult(new List<User>());

        public Task<List<User>> GetUsersAsync(int projectId) =>
            Task.FromResult(new List<User>());
    }

    private sealed class NullLogger : ILogger
    {
        public void Log(LogLevel level, string msg) { }
    }
}
