using CustomizableTablesWithDeadlines.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Configurations;

public class NotificationRuleConfiguration : IEntityTypeConfiguration<NotificationRule>
{
    public void Configure(EntityTypeBuilder<NotificationRule> builder)
    {
        builder.ToTable("NotificationRules");
        builder.HasKey(r => r.Id);

        builder.HasMany(r => r.NotificationLogs)
            .WithOne(l => l.NotificationRule)
            .HasForeignKey(l => l.NotificationRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
