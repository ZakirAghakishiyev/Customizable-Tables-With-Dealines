namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface IDesktopNotificationService
{
    Task ShowNotificationAsync(string title, string message, CancellationToken cancellationToken = default);
    Task<bool> ShowTestNotificationAsync(string title, string message, CancellationToken cancellationToken = default);
}
