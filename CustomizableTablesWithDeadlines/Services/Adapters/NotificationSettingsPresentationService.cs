using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.DTOs.Settings;
using CustomizableTablesWithDeadlines.Mapping;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.Services.Adapters;

public class NotificationSettingsPresentationService : INotificationSettingsService
{
    private readonly ISettingsService _settingsService;

    public NotificationSettingsPresentationService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<NotificationSettingsModel> GetSettingsAsync()
    {
        var settings = await _settingsService.GetAsync();
        return new NotificationSettingsModel
        {
            DefaultNotifyBefore = PresentationMapping.MinutesToNotifyOption(settings.DefaultNotifyBeforeMinutes),
            DefaultCustomNotifyMinutes = settings.DefaultNotifyBeforeMinutes,
            EnableDesktopNotifications = settings.EnableDesktopNotifications,
            EnableSound = settings.EnableSound,
            StartWithWindows = settings.StartWithWindows
        };
    }

    public async Task SaveSettingsAsync(NotificationSettingsModel settings)
    {
        var current = await _settingsService.GetAsync();
        await _settingsService.UpdateAsync(new AppSettingsDto
        {
            Language = current.Language,
            DefaultNotifyBeforeMinutes = PresentationMapping.NotifyOptionToMinutes(
                settings.DefaultNotifyBefore,
                settings.DefaultCustomNotifyMinutes),
            EnableDesktopNotifications = settings.EnableDesktopNotifications,
            EnableSound = settings.EnableSound,
            StartWithWindows = settings.StartWithWindows,
            Theme = current.Theme
        });
    }
}
