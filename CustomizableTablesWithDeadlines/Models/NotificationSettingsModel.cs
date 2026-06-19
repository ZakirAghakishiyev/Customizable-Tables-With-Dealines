using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Models;

public class NotificationSettingsModel
{
    public NotifyBeforeOption DefaultNotifyBefore { get; set; } = NotifyBeforeOption.OneHour;
    public int? DefaultCustomNotifyMinutes { get; set; }
    public bool EnableDesktopNotifications { get; set; } = true;
    public bool EnableSound { get; set; } = true;
    public bool StartWithWindows { get; set; }
}
