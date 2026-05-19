using client.Application.Interfaces;

namespace client.Presentation.Strategies;

public class SortByStatus : ITaskSortStrategy
{
    public void SortTasks(List<ITask> tasks)
    {
        tasks.Sort((task1, task2) => task1.Progress.CompareTo(task2.Progress));
    }
}
