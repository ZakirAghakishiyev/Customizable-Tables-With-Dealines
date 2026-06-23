using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.DTOs.Notifications;
using CustomizableTablesWithDeadlines.Application.Exceptions;
using CustomizableTablesWithDeadlines.Application.Validators;
using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Services;

public class NotificationRuleService : INotificationRuleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationScheduler _notificationScheduler;

    public NotificationRuleService(IUnitOfWork unitOfWork, INotificationScheduler notificationScheduler)
    {
        _unitOfWork = unitOfWork;
        _notificationScheduler = notificationScheduler;
    }

    public async Task<int> CreateAsync(CreateNotificationRuleDto dto, CancellationToken cancellationToken = default)
    {
        NotificationRuleValidator.ValidateNotifyBeforeMinutes(dto.NotifyBeforeMinutes);

        var deadline = await _unitOfWork.Deadlines.GetByIdAsync(dto.DeadlineId, cancellationToken)
                       ?? throw new NotFoundException(nameof(Deadline), dto.DeadlineId);

        var rule = new NotificationRule
        {
            DeadlineId = deadline.Id,
            NotifyBeforeMinutes = dto.NotifyBeforeMinutes,
            IsEnabled = dto.IsEnabled
        };

        await _unitOfWork.NotificationRules.AddAsync(rule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationScheduler.RescheduleDeadlineNotificationsAsync(deadline.Id, cancellationToken);
        return rule.Id;
    }

    public async Task UpdateAsync(NotificationRuleDto dto, CancellationToken cancellationToken = default)
    {
        NotificationRuleValidator.ValidateNotifyBeforeMinutes(dto.NotifyBeforeMinutes);

        var rule = await _unitOfWork.NotificationRules.GetByIdAsync(dto.Id, cancellationToken)
                   ?? throw new NotFoundException(nameof(NotificationRule), dto.Id);

        rule.NotifyBeforeMinutes = dto.NotifyBeforeMinutes;
        rule.IsEnabled = dto.IsEnabled;
        _unitOfWork.NotificationRules.Update(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationScheduler.RescheduleDeadlineNotificationsAsync(rule.DeadlineId, cancellationToken);
    }

    public async Task DeleteAsync(int ruleId, CancellationToken cancellationToken = default)
    {
        var rule = await _unitOfWork.NotificationRules.GetByIdAsync(ruleId, cancellationToken)
                   ?? throw new NotFoundException(nameof(NotificationRule), ruleId);

        var deadlineId = rule.DeadlineId;
        _unitOfWork.NotificationRules.Delete(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationScheduler.RescheduleDeadlineNotificationsAsync(deadlineId, cancellationToken);
    }

    public async Task EnableAsync(int ruleId, CancellationToken cancellationToken = default)
    {
        var rule = await _unitOfWork.NotificationRules.GetByIdAsync(ruleId, cancellationToken)
                   ?? throw new NotFoundException(nameof(NotificationRule), ruleId);

        rule.IsEnabled = true;
        _unitOfWork.NotificationRules.Update(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationScheduler.RescheduleDeadlineNotificationsAsync(rule.DeadlineId, cancellationToken);
    }

    public async Task DisableAsync(int ruleId, CancellationToken cancellationToken = default)
    {
        var rule = await _unitOfWork.NotificationRules.GetByIdAsync(ruleId, cancellationToken)
                   ?? throw new NotFoundException(nameof(NotificationRule), ruleId);

        rule.IsEnabled = false;
        _unitOfWork.NotificationRules.Update(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationScheduler.RescheduleDeadlineNotificationsAsync(rule.DeadlineId, cancellationToken);
    }
}
