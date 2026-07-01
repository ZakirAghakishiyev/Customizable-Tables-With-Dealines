using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class NotificationSettingsViewModel : LocalizedViewModelBase
{
    private readonly INotificationSettingsService _settingsService;
    private readonly IDesktopNotificationService _notificationService;

    [ObservableProperty] private NotifyBeforeOption _defaultNotifyBefore = NotifyBeforeOption.OneHour;
    [ObservableProperty] private int _defaultCustomNotifyMinutes = 30;
    [ObservableProperty] private bool _enableDesktopNotifications = true;
    [ObservableProperty] private bool _enableSound = true;
    [ObservableProperty] private bool _startWithWindows;

    public Array NotifyOptions => Enum.GetValues<NotifyBeforeOption>();

    public NotificationSettingsViewModel(
        ILocalizationService localization,
        INotificationSettingsService settingsService,
        IDesktopNotificationService notificationService) : base(localization)
    {
        _settingsService = settingsService;
        _notificationService = notificationService;
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

    [RelayCommand]
    private async Task TestNotificationAsync()
    {
        var shown = await _notificationService.ShowTestNotificationAsync(
            Strings.TestNotificationTitle,
            Strings.TestNotificationMessage);

        if (shown)
            MessageBoxHelper.ShowInfo(Strings.TestNotificationSent);
        else
            MessageBoxHelper.ShowWarning(Strings.TestNotificationFailed);
    }

    public string GetNotifyOptionText(NotifyBeforeOption option) => Strings.GetNotifyBeforeText(option);
}
