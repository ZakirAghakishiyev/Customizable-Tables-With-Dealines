using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CustomizableTablesWithDeadlines.Infrastructure.Scheduling;

[DisallowConcurrentExecution]
public class DeadlineNotificationJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var deadlineId = context.MergedJobDataMap.GetInt("DeadlineId");
        var notificationRuleId = context.MergedJobDataMap.GetInt("NotificationRuleId");
        var notificationLogId = context.MergedJobDataMap.GetInt("NotificationLogId");

        var scopeFactory = context.Scheduler.Context["ServiceProvider"] as IServiceScopeFactory;
        if (scopeFactory is null)
            throw new InvalidOperationException("Service provider is not available to the Quartz scheduler.");

        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var notificationService = scope.ServiceProvider.GetRequiredService<IDesktopNotificationService>();

        try
        {
            var deadline = await unitOfWork.Deadlines.GetByIdAsync(deadlineId, context.CancellationToken);
            if (deadline is null)
            {
                await unitOfWork.Notifications.MarkAsFailedAsync(notificationLogId, context.CancellationToken);
                await unitOfWork.SaveChangesAsync(context.CancellationToken);
                return;
            }

            var title = "Deadline Reminder";
            var message = $"{deadline.Title} is due at {deadline.DeadlineDateTime:g}";
            await notificationService.ShowNotificationAsync(title, message, context.CancellationToken);
            await unitOfWork.Notifications.MarkAsSentAsync(notificationLogId, context.CancellationToken);
            await unitOfWork.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            await unitOfWork.Notifications.MarkAsFailedAsync(notificationLogId, context.CancellationToken);
            await unitOfWork.SaveChangesAsync(context.CancellationToken);

            var logger = scope.ServiceProvider.GetService<ILogger<DeadlineNotificationJob>>();
            logger?.LogError(ex, "Failed to send deadline notification {NotificationLogId}.", notificationLogId);
        }
    }
}
