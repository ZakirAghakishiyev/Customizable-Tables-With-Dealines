namespace CustomizableTablesWithDeadlines.Application.Abstractions.Infrastructure;

public interface IPathProvider
{
    string GetAppDataDirectory();
    string GetDatabasePath();
    string GetSettingsPath();
}
