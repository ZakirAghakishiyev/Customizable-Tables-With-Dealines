using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;

public interface IDeadlineRepository : IRepository<Deadline>
{
    Task<IReadOnlyList<Deadline>> GetAllWithRowAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Deadline>> GetUpcomingDeadlinesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Deadline>> GetOverdueDeadlinesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Deadline>> GetDeadlinesByRowIdAsync(int rowId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Deadline>> GetAllActiveFutureDeadlinesAsync(CancellationToken cancellationToken = default);
    Task<Deadline?> GetWithNotificationRulesAsync(int deadlineId, CancellationToken cancellationToken = default);
}
