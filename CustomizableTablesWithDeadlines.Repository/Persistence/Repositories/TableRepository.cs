using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Repositories;

public class TableRepository : Repository<Table>, ITableRepository
{
    public TableRepository(AppDbContext context) : base(context)
    {
    }

    public Task<Table?> GetTableWithColumnsAsync(int tableId, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(t => t.Columns.OrderBy(c => c.OrderIndex))
            .FirstOrDefaultAsync(t => t.Id == tableId, cancellationToken);

    public Task<Table?> GetTableWithRowsAndCellsAsync(int tableId, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(t => t.Columns.OrderBy(c => c.OrderIndex))
            .Include(t => t.Rows.OrderBy(r => r.OrderNumber))
                .ThenInclude(r => r.CellValues)
            .FirstOrDefaultAsync(t => t.Id == tableId, cancellationToken);

    public Task<Table?> GetFullTableAsync(int tableId, CancellationToken cancellationToken = default) =>
        DbSet
            .AsNoTracking()
            .Include(t => t.Columns.OrderBy(c => c.OrderIndex))
            .Include(t => t.Rows.OrderBy(r => r.OrderNumber))
                .ThenInclude(r => r.CellValues)
            .Include(t => t.Rows)
                .ThenInclude(r => r.Deadlines)
                    .ThenInclude(d => d.NotificationRules)
            .Include(t => t.Rows)
                .ThenInclude(r => r.Deadlines)
                    .ThenInclude(d => d.NotificationLogs)
            .FirstOrDefaultAsync(t => t.Id == tableId, cancellationToken);
}
