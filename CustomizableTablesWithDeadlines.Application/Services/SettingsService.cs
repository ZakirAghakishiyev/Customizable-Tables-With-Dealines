using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.Common;
using CustomizableTablesWithDeadlines.Application.DTOs.Settings;
using CustomizableTablesWithDeadlines.Application.Validators;

namespace CustomizableTablesWithDeadlines.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly IAppSettingsService _appSettingsService;

    public SettingsService(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService;
    }

    public async Task<AppSettingsDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _appSettingsService.GetSettingsAsync(cancellationToken);
        return settings.ToDto();
    }

    public async Task UpdateAsync(AppSettingsDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Language))
            throw new Exceptions.ValidationException("Language is required.");

        NotificationRuleValidator.ValidateNotifyBeforeMinutes(dto.DefaultNotifyBeforeMinutes);

        await _appSettingsService.SaveSettingsAsync(dto.ToEntity(), cancellationToken);
    }
}
