using System.Text.Json;
using CustomizableTablesWithDeadlines.Application.Abstractions.Infrastructure;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.Settings;

namespace CustomizableTablesWithDeadlines.Infrastructure.Settings;

public class JsonAppSettingsService : IAppSettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IPathProvider _pathProvider;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public JsonAppSettingsService(IPathProvider pathProvider)
    {
        _pathProvider = pathProvider;
    }

    public async Task<AppSettings> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var path = _pathProvider.GetSettingsPath();
            if (!File.Exists(path))
                return new AppSettings();

            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonOptions, cancellationToken)
                   ?? new AppSettings();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        await _lock.WaitAsync(cancellationToken);
        try
        {
            var directory = _pathProvider.GetAppDataDirectory();
            Directory.CreateDirectory(directory);

            var path = _pathProvider.GetSettingsPath();
            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, settings, JsonOptions, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }
}
