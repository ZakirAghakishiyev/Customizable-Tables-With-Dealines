using CustomizableTablesWithDeadlines.Application.DTOs.Import;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface IWordImportService
{
    Task<IReadOnlyList<DetectedWordTableDto>> DetectTablesAsync(string filePath, CancellationToken cancellationToken = default);
    Task<ImportTableResultDto> ImportTableAsync(string filePath, int tableIndex, bool firstRowAsHeader, CancellationToken cancellationToken = default);
}
