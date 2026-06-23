using CustomizableTablesWithDeadlines.Application.DTOs.Columns;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface IColumnService
{
    Task<int> CreateAsync(CreateColumnDto dto, CancellationToken cancellationToken = default);
    Task RenameAsync(RenameColumnDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int columnId, CancellationToken cancellationToken = default);
    Task ReorderAsync(List<ReorderColumnDto> columns, CancellationToken cancellationToken = default);
}
