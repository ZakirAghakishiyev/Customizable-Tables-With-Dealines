using CustomizableTablesWithDeadlines.Models;

namespace CustomizableTablesWithDeadlines.Services.Interfaces;

public interface INotificationSettingsService
{
    Task<NotificationSettingsModel> GetSettingsAsync();
    Task SaveSettingsAsync(NotificationSettingsModel settings);
}
