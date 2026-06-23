using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    ITableRepository Tables { get; }
    IDeadlineRepository Deadlines { get; }
    INotificationRepository Notifications { get; }
    IRepository<Column> Columns { get; }
    IRepository<Row> Rows { get; }
    IRepository<CellValue> CellValues { get; }
    IRepository<NotificationRule> NotificationRules { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
