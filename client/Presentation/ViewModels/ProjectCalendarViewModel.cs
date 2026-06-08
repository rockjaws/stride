// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using System.Collections.ObjectModel;

using client.Presentation.Commands;
using client.Application.Interfaces;
using client.Presentation.Common;

namespace client.Presentation.ViewModels;

public class ProjectCalendarViewModel : ObservableObject
{
    private DateTime _displayMonth;
    private ObservableCollection<CalendarDayViewModel> _days = [];
    private HashSet<DateTime> _deadlineDates = [];

    public ObservableCollection<CalendarDayViewModel> Days
    {
        get => _days;
        private set => SetProperty(ref _days, value);
    }

    public string MonthLabel => _displayMonth.ToString("MMMM yyyy");

    public IUndoableCommand PreviousMonthCommand { get; }
    public IUndoableCommand NextMonthCommand { get; }

    public static IReadOnlyList<string> DayHeaders { get; } =
        ["Mo", "Tu", "We", "Th", "Fr", "Sa", "Su"];

    // Author: Nicolai
    public ProjectCalendarViewModel()
    {
        _displayMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        PreviousMonthCommand = new NavigateMonthCommand(this, -1);
        NextMonthCommand = new NavigateMonthCommand(this, 1);

        Rebuild();
    }

    // Author: Nicolai
    public void UpdateDeadlines(HashSet<DateTime> dates)
    {
        _deadlineDates = dates;
        Rebuild();
    }

    // Author: Nicolai
    internal void ShiftMonth(int direction)
    {
        _displayMonth = _displayMonth.AddMonths(direction);
        Rebuild();
    }

    // Author: Nicolai and Oliver
    private void Rebuild()
    {
        OnPropertyChanged(nameof(MonthLabel));

        var firstOfMonth = new DateTime(_displayMonth.Year, _displayMonth.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(_displayMonth.Year, _displayMonth.Month);
        // Convert Sunday-first DayOfWeek values into a Monday-first calendar offset.
        int leadingBlanks = ((int)firstOfMonth.DayOfWeek + 6) % 7;

        var cells = new List<CalendarDayViewModel>(42);

        // Always build a six-week grid so the calendar does not resize between months.
        for (int i = leadingBlanks - 1; i >= 0; i--)
            cells.Add(MakeDay(firstOfMonth.AddDays(-(i + 1)), isCurrentMonth: false));

        for (int d = 0; d < daysInMonth; d++)
            cells.Add(MakeDay(firstOfMonth.AddDays(d), isCurrentMonth: true));

        var firstOfNext = firstOfMonth.AddMonths(1);
        for (int i = 0; cells.Count < 42; i++)
            cells.Add(MakeDay(firstOfNext.AddDays(i), isCurrentMonth: false));

        Days = new ObservableCollection<CalendarDayViewModel>(cells);
    }

    // Author: Nicolai and Oliver
    private CalendarDayViewModel MakeDay(DateTime date, bool isCurrentMonth) => new()
    {
        Date = date,
        IsCurrentMonth = isCurrentMonth,
        IsToday = date.Date == DateTime.Today,
        // Dates are normalized before lookup so time-of-day never hides an indicator.
        HasTask = _deadlineDates.Contains(date.Date)
    };
}
