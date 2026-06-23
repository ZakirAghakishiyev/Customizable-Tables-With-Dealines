using CustomizableTablesWithDeadlines.Application.DTOs.Deadlines;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface IDeadlineService
{
    Task<List<DeadlineDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<DeadlineDto>> GetUpcomingAsync(CancellationToken cancellationToken = default);
    Task<List<DeadlineDto>> GetOverdueAsync(CancellationToken cancellationToken = default);
    Task<List<DeadlineDto>> GetByRowIdAsync(int rowId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateDeadlineDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateDeadlineDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int deadlineId, CancellationToken cancellationToken = default);
}
