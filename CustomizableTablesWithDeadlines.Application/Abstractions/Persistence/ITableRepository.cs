using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;

public interface ITableRepository : IRepository<Table>
{
    Task<Table?> GetTableWithColumnsAsync(int tableId, CancellationToken cancellationToken = default);
    Task<Table?> GetTableWithRowsAndCellsAsync(int tableId, CancellationToken cancellationToken = default);
    Task<Table?> GetFullTableAsync(int tableId, CancellationToken cancellationToken = default);
}
