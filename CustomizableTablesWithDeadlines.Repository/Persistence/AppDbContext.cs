using CustomizableTablesWithDeadlines.Domain.Entities;
using CustomizableTablesWithDeadlines.Domain.Entities.Base;
using CustomizableTablesWithDeadlines.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Column> Columns => Set<Column>();
    public DbSet<Row> Rows => Set<Row>();
    public DbSet<CellValue> CellValues => Set<CellValue>();
    public DbSet<Deadline> Deadlines => Set<Deadline>();
    public DbSet<NotificationRule> NotificationRules => Set<NotificationRule>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TableConfiguration());
        modelBuilder.ApplyConfiguration(new ColumnConfiguration());
        modelBuilder.ApplyConfiguration(new RowConfiguration());
        modelBuilder.ApplyConfiguration(new CellValueConfiguration());
        modelBuilder.ApplyConfiguration(new DeadlineConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationRuleConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationLogConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditedEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
