using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Domain.Entities;
using CustomizableTablesWithDeadlines.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace CustomizableTablesWithDeadlines.Infrastructure.Scheduling;

public class QuartzNotificationScheduler : INotificationScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IServiceScopeFactory _scopeFactory;

    public QuartzNotificationScheduler(ISchedulerFactory schedulerFactory, IServiceScopeFactory scopeFactory)
    {
        _schedulerFactory = schedulerFactory;
        _scopeFactory = scopeFactory;
    }

    public async Task ScheduleAllFutureNotificationsAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var scheduler = await GetStartedSchedulerAsync(cancellationToken);

        var deadlines = await unitOfWork.Deadlines.GetAllActiveFutureDeadlinesAsync(cancellationToken);
        foreach (var deadline in deadlines)
        {
            foreach (var rule in deadline.NotificationRules.Where(r => r.IsEnabled))
                await ScheduleRuleAsync(scheduler, unitOfWork, deadline, rule, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ScheduleDeadlineNotificationsAsync(int deadlineId, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var scheduler = await GetStartedSchedulerAsync(cancellationToken);

        var deadline = await unitOfWork.Deadlines.GetWithNotificationRulesAsync(deadlineId, cancellationToken);
        if (deadline is null)
            return;

        foreach (var rule in deadline.NotificationRules.Where(r => r.IsEnabled))
            await ScheduleRuleAsync(scheduler, unitOfWork, deadline, rule, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RescheduleDeadlineNotificationsAsync(int deadlineId, CancellationToken cancellationToken = default)
    {
        await CancelDeadlineNotificationsAsync(deadlineId, cancellationToken);
        await ScheduleDeadlineNotificationsAsync(deadlineId, cancellationToken);
    }

    public async Task CancelDeadlineNotificationsAsync(int deadlineId, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var scheduler = await GetStartedSchedulerAsync(cancellationToken);

        var deadline = await unitOfWork.Deadlines.GetWithNotificationRulesAsync(deadlineId, cancellationToken);
        if (deadline is not null)
        {
            foreach (var rule in deadline.NotificationRules)
            {
                var jobKey = NotificationJobKeys.CreateJobKey(deadlineId, rule.Id);
                if (await scheduler.CheckExists(jobKey, cancellationToken))
                    await scheduler.DeleteJob(jobKey, cancellationToken);
            }
        }

        await unitOfWork.Notifications.CancelPendingNotificationsForDeadlineAsync(deadlineId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ScheduleRuleAsync(
        IScheduler scheduler,
        IUnitOfWork unitOfWork,
        Deadline deadline,
        NotificationRule rule,
        CancellationToken cancellationToken)
    {
        var notificationTime = deadline.DeadlineDateTime.AddMinutes(-rule.NotifyBeforeMinutes);
        if (notificationTime <= DateTime.UtcNow)
            return;

        var jobKey = NotificationJobKeys.CreateJobKey(deadline.Id, rule.Id);
        if (await scheduler.CheckExists(jobKey, cancellationToken))
            await scheduler.DeleteJob(jobKey, cancellationToken);

        var existingLog = await unitOfWork.Notifications.GetPendingLogAsync(deadline.Id, rule.Id, cancellationToken);
        if (existingLog is not null)
            existingLog.Status = NotificationStatus.Cancelled;

        var log = new NotificationLog
        {
            DeadlineId = deadline.Id,
            NotificationRuleId = rule.Id,
            ScheduledFor = notificationTime,
            Status = NotificationStatus.Pending
        };
        await unitOfWork.Notifications.AddNotificationLogAsync(log, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var jobDetail = JobBuilder.Create<DeadlineNotificationJob>()
            .WithIdentity(jobKey)
            .UsingJobData("DeadlineId", deadline.Id)
            .UsingJobData("NotificationRuleId", rule.Id)
            .UsingJobData("NotificationLogId", log.Id)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(NotificationJobKeys.CreateTriggerKey(deadline.Id, rule.Id))
            .StartAt(new DateTimeOffset(notificationTime))
            .ForJob(jobDetail)
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
    }

    private async Task<IScheduler> GetStartedSchedulerAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        if (!scheduler.Context.ContainsKey("ServiceProvider"))
            scheduler.Context["ServiceProvider"] = _scopeFactory;

        if (!scheduler.IsStarted)
            await scheduler.Start(cancellationToken);

        return scheduler;
    }
}
