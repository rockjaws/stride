using client.Application.Interfaces;

namespace client.Presentation.Strategies;

public class SortByDeadline : ITaskSortStrategy
{
    public void SortTasks(List<ITask> tasks)
    {
        tasks.Sort((task1, task2) => task1.Deadline.CompareTo(task2.Deadline));
    }
}
