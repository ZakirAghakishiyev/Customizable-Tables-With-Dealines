using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.Services.Mock;

public class MockNotificationSettingsService : INotificationSettingsService
{
    private NotificationSettingsModel _settings = new();

    public Task<NotificationSettingsModel> GetSettingsAsync() =>
        Task.FromResult(_settings);

    public Task SaveSettingsAsync(NotificationSettingsModel settings)
    {
        _settings = settings;
        return Task.CompletedTask;
    }
}
