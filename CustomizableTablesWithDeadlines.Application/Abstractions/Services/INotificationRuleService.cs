using CustomizableTablesWithDeadlines.Application.DTOs.Notifications;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface INotificationRuleService
{
    Task<int> CreateAsync(CreateNotificationRuleDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationRuleDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int ruleId, CancellationToken cancellationToken = default);
    Task EnableAsync(int ruleId, CancellationToken cancellationToken = default);
    Task DisableAsync(int ruleId, CancellationToken cancellationToken = default);
}
