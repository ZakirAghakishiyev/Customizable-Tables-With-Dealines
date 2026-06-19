using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Services.Interfaces;

public interface IDeadlineService
{
    Task<IReadOnlyList<DeadlineListItem>> GetAllDeadlinesAsync();
    Task<IReadOnlyList<DeadlineListItem>> GetUpcomingDeadlinesAsync(int count);
    Task<int> GetUpcomingCountAsync();
    Task<int> GetOverdueCountAsync();
    DeadlineFilter ApplyFilter(IEnumerable<DeadlineListItem> deadlines, DeadlineFilter filter);
}
