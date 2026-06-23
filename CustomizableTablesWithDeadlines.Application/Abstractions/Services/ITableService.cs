using CustomizableTablesWithDeadlines.Application.DTOs.Tables;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Services;

public interface ITableService
{
    Task<List<TableDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TableDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateTableDto dto, CancellationToken cancellationToken = default);
    Task RenameAsync(RenameTableDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
