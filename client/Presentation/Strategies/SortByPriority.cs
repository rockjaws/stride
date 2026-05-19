using client.Application.Interfaces;

namespace client.Presentation.Strategies;

public class SortByPriority : ITaskSortStrategy
{
    public void SortTasks(List<ITask> tasks)
    {
        tasks.Sort((task1, task2) => task1.Priority.CompareTo(task2.Priority));
    }
}
