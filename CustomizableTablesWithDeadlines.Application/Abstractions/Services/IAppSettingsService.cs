using CustomizableTablesWithDeadlines.Application.Settings;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface IAppSettingsService
{
    Task<AppSettings> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task SaveSettingsAsync(AppSettings settings, CancellationToken cancellationToken = default);
}
