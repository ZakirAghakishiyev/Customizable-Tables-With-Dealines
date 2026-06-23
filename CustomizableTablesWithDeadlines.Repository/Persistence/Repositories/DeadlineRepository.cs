using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Repositories;

public class DeadlineRepository : Repository<Deadline>, IDeadlineRepository
{
    public DeadlineRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Deadline>> GetAllWithRowAsync(CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(d => d.Row)
            .Include(d => d.NotificationRules)
            .OrderBy(d => d.DeadlineDateTime)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Deadline>> GetUpcomingDeadlinesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        return await DbSet
            .AsNoTracking()
            .Include(d => d.NotificationRules)
            .Where(d => d.DeadlineDateTime >= now)
            .OrderBy(d => d.DeadlineDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Deadline>> GetOverdueDeadlinesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        return await DbSet
            .AsNoTracking()
            .Include(d => d.NotificationRules)
            .Where(d => d.DeadlineDateTime < now)
            .OrderBy(d => d.DeadlineDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Deadline>> GetDeadlinesByRowIdAsync(int rowId, CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(d => d.NotificationRules)
            .Where(d => d.RowId == rowId)
            .OrderBy(d => d.DeadlineDateTime)
            .ToListAsync(cancellationToken);

    public Task<Deadline?> GetWithNotificationRulesAsync(int deadlineId, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(d => d.NotificationRules)
            .FirstOrDefaultAsync(d => d.Id == deadlineId, cancellationToken);

    public async Task<IReadOnlyList<Deadline>> GetAllActiveFutureDeadlinesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        return await DbSet
            .Include(d => d.NotificationRules)
            .Where(d => d.DeadlineDateTime > now)
            .Where(d => d.NotificationRules.Any(r => r.IsEnabled))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
