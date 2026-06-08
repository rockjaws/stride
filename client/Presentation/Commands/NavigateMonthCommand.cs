// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Presentation.ViewModels;

namespace client.Presentation.Commands;

public class NavigateMonthCommand : IUndoableCommand
{
    private readonly ProjectCalendarViewModel _calendarViewModel;
    private readonly int _direction; // -1 = previous, +1 = next

    // Author: Nicolai
    public NavigateMonthCommand(ProjectCalendarViewModel calendarViewModel, int direction)
    {
        _calendarViewModel = calendarViewModel;
        _direction = direction;
    }

    // Author: Nicolai
    public void Execute(object? parameter) => _calendarViewModel.ShiftMonth(_direction);

    // Author: Nicolai
    public void Undo() => _calendarViewModel.ShiftMonth(-_direction);

    // Author: Nicolai
    public bool CanExecute(object? parameter) => true;

    public event EventHandler? CanExecuteChanged;
}
