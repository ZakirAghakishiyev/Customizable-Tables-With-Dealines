using CustomizableTablesWithDeadlines.Domain.Entities;
using CustomizableTablesWithDeadlines.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Configurations;

public class ColumnConfiguration : IEntityTypeConfiguration<Column>
{
    public void Configure(EntityTypeBuilder<Column> builder)
    {
        builder.ToTable("Columns");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(256);
        builder.Property(c => c.DataType).HasConversion<int>();

        builder.HasMany(c => c.CellValues)
            .WithOne(cv => cv.Column)
            .HasForeignKey(cv => cv.ColumnId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
