using CommunityToolkit.WinUI.Notifications;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Infrastructure.Notifications;
using Microsoft.Extensions.Logging;
using System.Windows;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using WpfApplication = System.Windows.Application;

namespace CustomizableTablesWithDeadlines.Services;

public class WpfToastNotificationService : IDesktopNotificationService
{
    private readonly IAppSettingsService _settingsService;
    private readonly IDesktopNotificationFallback? _fallback;
    private readonly ILogger<WpfToastNotificationService> _logger;

    public WpfToastNotificationService(
        IAppSettingsService settingsService,
        ILogger<WpfToastNotificationService> logger,
        IDesktopNotificationFallback? fallback = null)
    {
        _settingsService = settingsService;
        _logger = logger;
        _fallback = fallback;
    }

    public async Task ShowNotificationAsync(string title, string message, CancellationToken cancellationToken = default)
    {
        _ = await ShowInternalAsync(title, message, respectSettings: true, useFallback: true, minimizeForBanner: false, cancellationToken);
    }

    public Task<bool> ShowTestNotificationAsync(string title, string message, CancellationToken cancellationToken = default) =>
        ShowInternalAsync(title, message, respectSettings: false, useFallback: true, minimizeForBanner: true, cancellationToken);

    private async Task<bool> ShowInternalAsync(
        string title,
        string message,
        bool respectSettings,
        bool useFallback,
        bool minimizeForBanner,
        CancellationToken cancellationToken)
    {
        if (respectSettings)
        {
            var settings = await _settingsService.GetSettingsAsync(cancellationToken);
            if (!settings.EnableDesktopNotifications)
                return false;
        }

        Window? mainWindow = null;
        var shouldRestoreWindow = false;

        try
        {
            if (minimizeForBanner)
            {
                mainWindow = WpfApplication.Current?.MainWindow;
                shouldRestoreWindow = await TryMinimizeActiveWindowAsync(cancellationToken);
            }

            var shown = await InvokeOnUiThreadAsync(() => TryShowToast(title, message), cancellationToken);
            if (shown)
            {
                if (shouldRestoreWindow)
                    _ = RestoreWindowAfterDelayAsync(mainWindow, cancellationToken);

                return true;
            }

            _logger.LogWarning("Desktop toast was not shown. Notifier may be disabled or unavailable.");
            if (useFallback && _fallback is not null)
            {
                _fallback.Show(title, message);
                if (shouldRestoreWindow)
                    _ = RestoreWindowAfterDelayAsync(mainWindow, cancellationToken);

                return true;
            }

            if (shouldRestoreWindow)
                _ = RestoreWindowAfterDelayAsync(mainWindow, cancellationToken);

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Windows toast notification failed.");
            if (useFallback && _fallback is not null)
            {
                _fallback.Show(title, message);
                if (shouldRestoreWindow)
                    _ = RestoreWindowAfterDelayAsync(mainWindow, cancellationToken);

                return true;
            }

            if (shouldRestoreWindow)
                _ = RestoreWindowAfterDelayAsync(mainWindow, cancellationToken);

            return false;
        }
    }

    private static bool TryShowToast(string title, string message)
    {
        WindowsToastRegistration.Register();

        var notifier = WindowsToastRegistration.CreateNotifier();
        if (notifier.Setting is NotificationSetting.DisabledForUser or NotificationSetting.DisabledByGroupPolicy)
            return false;

        var content = new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .SetToastScenario(ToastScenario.Reminder)
            .AddAudio(new Uri("ms-winsoundevent:Notification.Reminder"))
            .GetToastContent();

        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(content.GetContent());

        var toast = new ToastNotification(xmlDocument)
        {
            SuppressPopup = false,
            Priority = ToastNotificationPriority.High
        };

        notifier.Show(toast);
        return true;
    }

    private static async Task<bool> TryMinimizeActiveWindowAsync(CancellationToken cancellationToken)
    {
        var app = WpfApplication.Current;
        var mainWindow = app?.MainWindow;
        if (mainWindow is null || !mainWindow.IsActive)
            return false;

        if (!mainWindow.Dispatcher.CheckAccess())
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            mainWindow.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    tcs.TrySetResult(MinimizeWindow(mainWindow));
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            if (cancellationToken.CanBeCanceled)
                cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

            if (!await tcs.Task.ConfigureAwait(false))
                return false;
        }
        else if (!MinimizeWindow(mainWindow))
        {
            return false;
        }

        await Task.Delay(500, cancellationToken).ConfigureAwait(false);
        return true;
    }

    private static bool MinimizeWindow(Window mainWindow)
    {
        if (!mainWindow.IsActive)
            return false;

        mainWindow.WindowState = WindowState.Minimized;
        return true;
    }

    private static async Task RestoreWindowAfterDelayAsync(Window? mainWindow, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(2500, cancellationToken).ConfigureAwait(false);
            if (mainWindow is null)
                return;

            mainWindow.Dispatcher.BeginInvoke(() =>
            {
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            });
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation during restore delay.
        }
    }

    private static Task<bool> InvokeOnUiThreadAsync(Func<bool> action, CancellationToken cancellationToken)
    {
        var app = WpfApplication.Current;
        if (app?.Dispatcher is { } dispatcher)
        {
            if (dispatcher.CheckAccess())
                return Task.FromResult(action());

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            dispatcher.BeginInvoke(() =>
            {
                try
                {
                    tcs.TrySetResult(action());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            if (cancellationToken.CanBeCanceled)
                cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

            return tcs.Task;
        }

        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            return Task.FromResult(action());

        var backgroundTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            try
            {
                backgroundTcs.TrySetResult(action());
            }
            catch (Exception ex)
            {
                backgroundTcs.TrySetException(ex);
            }
        })
        {
            IsBackground = true
        };

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        if (cancellationToken.CanBeCanceled)
            cancellationToken.Register(() => backgroundTcs.TrySetCanceled(cancellationToken));

        return backgroundTcs.Task;
    }
}
