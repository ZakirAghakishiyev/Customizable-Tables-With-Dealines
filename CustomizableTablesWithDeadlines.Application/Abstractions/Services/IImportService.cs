using CustomizableTablesWithDeadlines.Application.DTOs.Import;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface IImportService
{
    Task<List<DetectedWordTableDto>> DetectWordTablesAsync(string filePath, CancellationToken cancellationToken = default);
    Task<int> ImportWordTableAsync(ImportTableRequestDto dto, CancellationToken cancellationToken = default);
}
