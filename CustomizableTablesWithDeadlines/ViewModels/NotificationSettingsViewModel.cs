using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class NotificationSettingsViewModel : LocalizedViewModelBase
{
    private readonly INotificationSettingsService _settingsService;

    [ObservableProperty] private NotifyBeforeOption _defaultNotifyBefore = NotifyBeforeOption.OneHour;
    [ObservableProperty] private int _defaultCustomNotifyMinutes = 30;
    [ObservableProperty] private bool _enableDesktopNotifications = true;
    [ObservableProperty] private bool _enableSound = true;
    [ObservableProperty] private bool _startWithWindows;

    public Array NotifyOptions => Enum.GetValues<NotifyBeforeOption>();

    public NotificationSettingsViewModel(
        ILocalizationService localization,
        INotificationSettingsService settingsService) : base(localization)
    {
        _settingsService = settingsService;
    }

    public async Task LoadAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        DefaultNotifyBefore = settings.DefaultNotifyBefore;
        DefaultCustomNotifyMinutes = settings.DefaultCustomNotifyMinutes ?? 30;
        EnableDesktopNotifications = settings.EnableDesktopNotifications;
        EnableSound = settings.EnableSound;
        StartWithWindows = settings.StartWithWindows;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await _settingsService.SaveSettingsAsync(new NotificationSettingsModel
        {
            DefaultNotifyBefore = DefaultNotifyBefore,
            DefaultCustomNotifyMinutes = DefaultCustomNotifyMinutes,
            EnableDesktopNotifications = EnableDesktopNotifications,
            EnableSound = EnableSound,
            StartWithWindows = StartWithWindows
        });
    }

    public string GetNotifyOptionText(NotifyBeforeOption option) => Strings.GetNotifyBeforeText(option);
}
