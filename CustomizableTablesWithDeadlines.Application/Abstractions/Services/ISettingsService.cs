using CustomizableTablesWithDeadlines.Application.DTOs.Settings;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface ISettingsService
{
    Task<AppSettingsDto> GetAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(AppSettingsDto dto, CancellationToken cancellationToken = default);
}
