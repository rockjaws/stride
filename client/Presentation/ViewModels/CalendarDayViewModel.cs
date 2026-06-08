// =============================================================================
// Author: Nicolai
// =============================================================================

namespace client.Presentation.ViewModels;

public class CalendarDayViewModel
{
    public DateTime Date { get; init; }
    public bool IsCurrentMonth { get; init; }
    public bool IsToday { get; init; }
    public bool HasTask { get; init; }
    public int DayNumber => Date.Day;
}
