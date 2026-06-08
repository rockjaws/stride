// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using client.Application.Interfaces;

namespace client.Presentation.Strategies;

public interface ITaskSortStrategy
{
    // Author: Nicolai and Oliver
    void SortTasks(List<ITask> tasks);
}
