using CustomizableTablesWithDeadlines.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Configurations;

public class DeadlineConfiguration : IEntityTypeConfiguration<Deadline>
{
    public void Configure(EntityTypeBuilder<Deadline> builder)
    {
        builder.ToTable("Deadlines");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Title).IsRequired().HasMaxLength(512);
        builder.HasIndex(d => d.DeadlineDateTime);

        builder.HasMany(d => d.NotificationRules)
            .WithOne(r => r.Deadline)
            .HasForeignKey(r => r.DeadlineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.NotificationLogs)
            .WithOne(l => l.Deadline)
            .HasForeignKey(l => l.DeadlineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
