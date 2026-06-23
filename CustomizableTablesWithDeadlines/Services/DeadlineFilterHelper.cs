using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Services;

public static class DeadlineFilterHelper
{
    public static IEnumerable<DeadlineListItem> Filter(
        IEnumerable<DeadlineListItem> deadlines,
        DeadlineFilter filter)
    {
        var today = DateTime.Today;
        var weekEnd = today.AddDays(7);

        return filter switch
        {
            DeadlineFilter.Today => deadlines.Where(d => d.DeadlineDateTime.Date == today),
            DeadlineFilter.ThisWeek => deadlines.Where(d =>
                d.DeadlineDateTime.Date >= today && d.DeadlineDateTime.Date < weekEnd),
            DeadlineFilter.Overdue => deadlines.Where(d =>
                !d.IsCompleted && d.DeadlineDateTime < DateTime.Now),
            _ => deadlines
        };
    }
}
