namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface INotificationScheduler
{
    Task ScheduleAllFutureNotificationsAsync(CancellationToken cancellationToken = default);
    Task ScheduleDeadlineNotificationsAsync(int deadlineId, CancellationToken cancellationToken = default);
    Task RescheduleDeadlineNotificationsAsync(int deadlineId, CancellationToken cancellationToken = default);
    Task CancelDeadlineNotificationsAsync(int deadlineId, CancellationToken cancellationToken = default);
}
