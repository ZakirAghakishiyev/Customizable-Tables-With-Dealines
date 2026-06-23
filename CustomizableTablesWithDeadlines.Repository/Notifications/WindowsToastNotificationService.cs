using CommunityToolkit.WinUI.Notifications;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace CustomizableTablesWithDeadlines.Infrastructure.Notifications;

public class WindowsToastNotificationService : IDesktopNotificationService
{
    private readonly IAppSettingsService _settingsService;
    private readonly IDesktopNotificationFallback? _fallback;
    private readonly ILogger<WindowsToastNotificationService> _logger;

    public WindowsToastNotificationService(
        IAppSettingsService settingsService,
        ILogger<WindowsToastNotificationService> logger,
        IDesktopNotificationFallback? fallback = null)
    {
        _settingsService = settingsService;
        _logger = logger;
        _fallback = fallback;
    }

    public async Task ShowNotificationAsync(string title, string message, CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetSettingsAsync(cancellationToken);
        if (!settings.EnableDesktopNotifications)
            return;

        try
        {
            await Task.Run(() =>
            {
                var content = new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .GetToastContent();

                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(content.GetContent());

                var toast = new ToastNotification(xmlDocument);
                WindowsToastRegistration.CreateNotifier().Show(toast);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Windows toast notification failed.");
            _fallback?.Show(title, message);
        }
    }
}
