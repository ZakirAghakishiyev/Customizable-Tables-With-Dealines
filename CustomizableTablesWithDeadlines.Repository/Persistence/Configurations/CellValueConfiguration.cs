using CustomizableTablesWithDeadlines.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Configurations;

public class CellValueConfiguration : IEntityTypeConfiguration<CellValue>
{
    public void Configure(EntityTypeBuilder<CellValue> builder)
    {
        builder.ToTable("CellValues");
        builder.HasKey(cv => cv.Id);
        builder.Property(cv => cv.ValueNumber).HasColumnType("decimal(18,4)");
        builder.Property(cv => cv.ValueText).HasMaxLength(4000);

        builder.HasIndex(cv => new { cv.RowId, cv.ColumnId }).IsUnique();
    }
}
