// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Presentation.ViewModels;

namespace client.Presentation.Commands;

public class NavigateMonthCommand : IUndoableCommand
{
    private readonly ProjectCalendarViewModel _calendarViewModel;
    private readonly int _direction; // -1 = previous, +1 = next

    // Author: Nicolaj
    public NavigateMonthCommand(ProjectCalendarViewModel calendarViewModel, int direction)
    {
        _calendarViewModel = calendarViewModel;
        _direction = direction;
    }

    // Author: Nicolaj
    public void Execute(object? parameter) => _calendarViewModel.ShiftMonth(_direction);

    // Author: Nicolaj
    public void Undo() => _calendarViewModel.ShiftMonth(-_direction);

    // Author: Nicolaj
    public bool CanExecute(object? parameter) => true;

    public event EventHandler? CanExecuteChanged;
}
