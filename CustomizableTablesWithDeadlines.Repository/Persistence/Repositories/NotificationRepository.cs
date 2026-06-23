using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Domain.Entities;
using CustomizableTablesWithDeadlines.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<NotificationLog>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default) =>
        await _context.NotificationLogs
            .AsNoTracking()
            .Where(l => l.Status == NotificationStatus.Pending)
            .OrderBy(l => l.ScheduledFor)
            .ToListAsync(cancellationToken);

    public async Task<NotificationLog> AddNotificationLogAsync(NotificationLog log, CancellationToken cancellationToken = default)
    {
        await _context.NotificationLogs.AddAsync(log, cancellationToken);
        return log;
    }

    public async Task MarkAsSentAsync(int notificationLogId, CancellationToken cancellationToken = default)
    {
        var log = await _context.NotificationLogs.FindAsync([notificationLogId], cancellationToken);
        if (log is null)
            return;

        log.Status = NotificationStatus.Sent;
        log.SentAt = DateTime.UtcNow;
    }

    public async Task MarkAsFailedAsync(int notificationLogId, CancellationToken cancellationToken = default)
    {
        var log = await _context.NotificationLogs.FindAsync([notificationLogId], cancellationToken);
        if (log is null)
            return;

        log.Status = NotificationStatus.Failed;
    }

    public async Task CancelPendingNotificationsForDeadlineAsync(int deadlineId, CancellationToken cancellationToken = default)
    {
        var pendingLogs = await _context.NotificationLogs
            .Where(l => l.DeadlineId == deadlineId && l.Status == NotificationStatus.Pending)
            .ToListAsync(cancellationToken);

        foreach (var log in pendingLogs)
            log.Status = NotificationStatus.Cancelled;
    }

    public Task<NotificationLog?> GetPendingLogAsync(int deadlineId, int notificationRuleId, CancellationToken cancellationToken = default) =>
        _context.NotificationLogs
            .FirstOrDefaultAsync(
                l => l.DeadlineId == deadlineId
                     && l.NotificationRuleId == notificationRuleId
                     && l.Status == NotificationStatus.Pending,
                cancellationToken);
}
