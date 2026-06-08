// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Presentation.Algorithms;

namespace client.Presentation.Strategies;

public class SortByDeadline : ITaskSortStrategy
{
    // Author: Nicolaj and Oliver
    public void SortTasks(List<ITask> tasks)
    {
        if (tasks == null || tasks.Count <= 1) return;

        tasks.MergeSort(0, tasks.Count - 1, (task1, task2) => task1.Deadline.CompareTo(task2.Deadline));
    }
}
