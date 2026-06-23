using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;

public interface INotificationRepository
{
    Task<IReadOnlyList<NotificationLog>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default);
    Task<NotificationLog> AddNotificationLogAsync(NotificationLog log, CancellationToken cancellationToken = default);
    Task MarkAsSentAsync(int notificationLogId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(int notificationLogId, CancellationToken cancellationToken = default);
    Task CancelPendingNotificationsForDeadlineAsync(int deadlineId, CancellationToken cancellationToken = default);
    Task<NotificationLog?> GetPendingLogAsync(int deadlineId, int notificationRuleId, CancellationToken cancellationToken = default);
}
