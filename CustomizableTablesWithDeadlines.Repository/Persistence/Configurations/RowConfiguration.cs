using CustomizableTablesWithDeadlines.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Configurations;

public class RowConfiguration : IEntityTypeConfiguration<Row>
{
    public void Configure(EntityTypeBuilder<Row> builder)
    {
        builder.ToTable("Rows");
        builder.HasKey(r => r.Id);

        builder.HasMany(r => r.CellValues)
            .WithOne(cv => cv.Row)
            .HasForeignKey(cv => cv.RowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Deadlines)
            .WithOne(d => d.Row)
            .HasForeignKey(d => d.RowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
