namespace CustomizableTablesWithDeadlines.Application.DTOs.Settings;

public class AppSettingsDto
{
    public string Language { get; set; } = "en";
    public int DefaultNotifyBeforeMinutes { get; set; } = 60;
    public bool EnableDesktopNotifications { get; set; } = true;
    public bool EnableSound { get; set; } = true;
    public bool StartWithWindows { get; set; }
    public string Theme { get; set; } = "Light";
}
