using System.Windows;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;
using WpfApplication = System.Windows.Application;

namespace CustomizableTablesWithDeadlines.Services;

public class WpfDesktopNotificationFallback : IDesktopNotificationFallback
{
    private readonly ILogger<WpfDesktopNotificationFallback> _logger;

    public WpfDesktopNotificationFallback(ILogger<WpfDesktopNotificationFallback> logger)
    {
        _logger = logger;
    }

    public void Show(string title, string message)
    {
        _logger.LogInformation("Desktop notification fallback: {Title} — {Message}", title, message);

        var app = WpfApplication.Current;
        if (app is null)
            return;

        app.Dispatcher.BeginInvoke(() =>
        {
            var window = app.MainWindow;
            if (window is not null)
            {
                window.Activate();
                if (window.WindowState == WindowState.Minimized)
                    window.WindowState = WindowState.Normal;
            }

            MessageBox.Show(
                window,
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        });
    }
}
