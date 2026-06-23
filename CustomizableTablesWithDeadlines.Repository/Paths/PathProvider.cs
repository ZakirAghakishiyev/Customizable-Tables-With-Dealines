using CustomizableTablesWithDeadlines.Application.Abstractions.Infrastructure;

namespace CustomizableTablesWithDeadlines.Infrastructure.Paths;

public class PathProvider : IPathProvider
{
    private const string AppFolderName = "DeadlineManager";
    private const string DatabaseFileName = "deadlinemanager.db";
    private const string SettingsFileName = "settings.json";

    public string GetAppDataDirectory() =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppFolderName);

    public string GetDatabasePath() =>
        Path.Combine(GetAppDataDirectory(), DatabaseFileName);

    public string GetSettingsPath() =>
        Path.Combine(GetAppDataDirectory(), SettingsFileName);
}
