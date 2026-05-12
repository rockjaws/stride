using client.Application.Interfaces;

namespace client.Presentation.Strategies;

public interface ITaskSortStrategy
{
    void SortTasks(List<ITask> tasks) { }
}
