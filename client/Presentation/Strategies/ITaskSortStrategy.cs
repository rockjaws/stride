// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Application.Interfaces;

namespace client.Presentation.Strategies;

public interface ITaskSortStrategy
{
    // Author: Nicolaj and Oliver
    void SortTasks(List<ITask> tasks);
}
