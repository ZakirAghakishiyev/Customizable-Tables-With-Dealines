using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Infrastructure.Persistence.Repositories;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(
        AppDbContext context,
        ITableRepository tables,
        IDeadlineRepository deadlines,
        INotificationRepository notifications,
        IRepository<Domain.Entities.Column> columns,
        IRepository<Domain.Entities.Row> rows,
        IRepository<Domain.Entities.CellValue> cellValues,
        IRepository<Domain.Entities.NotificationRule> notificationRules)
    {
        _context = context;
        Tables = tables;
        Deadlines = deadlines;
        Notifications = notifications;
        Columns = columns;
        Rows = rows;
        CellValues = cellValues;
        NotificationRules = notificationRules;
    }

    public ITableRepository Tables { get; }
    public IDeadlineRepository Deadlines { get; }
    public INotificationRepository Notifications { get; }
    public IRepository<Domain.Entities.Column> Columns { get; }
    public IRepository<Domain.Entities.Row> Rows { get; }
    public IRepository<Domain.Entities.CellValue> CellValues { get; }
    public IRepository<Domain.Entities.NotificationRule> NotificationRules { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
