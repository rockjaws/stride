// =============================================================================
// Author: Oliver
// =============================================================================

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using client.Domain.Enum;

namespace client.Presentation.Converters;

public class TaskBadgeBrushConverter : IValueConverter
{
    // Author: Oliver
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // XAML passes "background", "border", or "foreground" to reuse the same status palette.
        string tone = parameter as string ?? "background";

        return value switch
        {
            TaskProgress.Backlog => BrushFor(tone, "#F8FAFC", "#64748B", "#334155"),
            TaskProgress.InProgress => BrushFor(tone, "#FFFBEB", "#D97706", "#92400E"),
            TaskProgress.Review => BrushFor(tone, "#F5F3FF", "#7C3AED", "#5B21B6"),
            TaskProgress.Done => BrushFor(tone, "#F0FDF4", "#16A34A", "#166534"),
            TaskPriority.Low => BrushFor(tone, "#F0FDF4", "#16A34A", "#166534"),
            TaskPriority.Normal => BrushFor(tone, "#EFF6FF", "#2563EB", "#1E40AF"),
            TaskPriority.High => BrushFor(tone, "#FEF2F2", "#DC2626", "#991B1B"),
            _ => BrushFor(tone, "#F8FAFC", "#CBD5E1", "#334155"),
        };
    }

    // Author: Oliver
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    // Author: Oliver
    private static Brush BrushFor(string tone, string background, string border, string foreground)
    {
        var color = tone switch
        {
            "border" => border,
            "foreground" => foreground,
            _ => background,
        };

        return (Brush)new BrushConverter().ConvertFromString(color)!;
    }
}
