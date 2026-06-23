namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface IDesktopNotificationFallback
{
    void Show(string title, string message);
}
