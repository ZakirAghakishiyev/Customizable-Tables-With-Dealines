using CustomizableTablesWithDeadlines.Application.DTOs.Rows;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface IRowService
{
    Task<int> CreateAsync(CreateRowDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int rowId, CancellationToken cancellationToken = default);
    Task UpdateCellValueAsync(UpdateCellValueDto dto, CancellationToken cancellationToken = default);
}
