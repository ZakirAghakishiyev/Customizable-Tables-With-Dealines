using CustomizableTablesWithDeadlines.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("Tables");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(256);

        builder.HasMany(t => t.Columns)
            .WithOne(c => c.Table)
            .HasForeignKey(c => c.TableId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Rows)
            .WithOne(r => r.Table)
            .HasForeignKey(r => r.TableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
