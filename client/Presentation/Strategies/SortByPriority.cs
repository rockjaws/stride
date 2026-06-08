// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Presentation.Algorithms;

namespace client.Presentation.Strategies;

public class SortByPriority : ITaskSortStrategy
{
    // Author: Nicolaj and Oliver
    public void SortTasks(List<ITask> tasks)
    {
        if (tasks == null || tasks.Count <= 1) return;

        tasks.MergeSort(0, tasks.Count - 1, (task1, task2) => task2.Priority.CompareTo(task1.Priority));
    }
}
