using CustomizableTablesWithDeadlines.Domain.Entities;
using CustomizableTablesWithDeadlines.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("NotificationLogs");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Status).HasConversion<int>();
        builder.HasIndex(l => l.ScheduledFor);
        builder.HasIndex(l => l.Status);
    }
}
