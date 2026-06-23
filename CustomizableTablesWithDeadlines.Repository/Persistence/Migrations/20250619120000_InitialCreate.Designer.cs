using System;
using CustomizableTablesWithDeadlines.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20250619120000_InitialCreate")]
public partial class InitialCreate
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "10.0.9");

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.CellValue", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<int>("ColumnId").HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<int>("RowId").HasColumnType("INTEGER");
            b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
            b.Property<bool?>("ValueBoolean").HasColumnType("INTEGER");
            b.Property<DateTime?>("ValueDateTime").HasColumnType("TEXT");
            b.Property<decimal?>("ValueNumber").HasColumnType("decimal(18,4)");
            b.Property<string>("ValueText").HasMaxLength(4000).HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("ColumnId");
            b.HasIndex("RowId", "ColumnId").IsUnique();
            b.ToTable("CellValues", (string)null);
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Column", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<int>("DataType").HasColumnType("INTEGER");
            b.Property<bool>("IsRequired").HasColumnType("INTEGER");
            b.Property<string>("Name").IsRequired().HasMaxLength(256).HasColumnType("TEXT");
            b.Property<int>("OrderIndex").HasColumnType("INTEGER");
            b.Property<int>("TableId").HasColumnType("INTEGER");
            b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("TableId");
            b.ToTable("Columns", (string)null);
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Deadline", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<DateTime>("DeadlineDateTime").HasColumnType("TEXT");
            b.Property<int>("RowId").HasColumnType("INTEGER");
            b.Property<string>("Title").IsRequired().HasMaxLength(512).HasColumnType("TEXT");
            b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("DeadlineDateTime");
            b.HasIndex("RowId");
            b.ToTable("Deadlines", (string)null);
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.NotificationLog", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<int>("DeadlineId").HasColumnType("INTEGER");
            b.Property<int>("NotificationRuleId").HasColumnType("INTEGER");
            b.Property<DateTime>("ScheduledFor").HasColumnType("TEXT");
            b.Property<DateTime?>("SentAt").HasColumnType("TEXT");
            b.Property<int>("Status").HasColumnType("INTEGER");
            b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("DeadlineId");
            b.HasIndex("NotificationRuleId");
            b.HasIndex("ScheduledFor");
            b.HasIndex("Status");
            b.ToTable("NotificationLogs", (string)null);
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.NotificationRule", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<int>("DeadlineId").HasColumnType("INTEGER");
            b.Property<bool>("IsEnabled").HasColumnType("INTEGER");
            b.Property<int>("NotifyBeforeMinutes").HasColumnType("INTEGER");
            b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("DeadlineId");
            b.ToTable("NotificationRules", (string)null);
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Row", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<int>("OrderNumber").HasColumnType("INTEGER");
            b.Property<int>("TableId").HasColumnType("INTEGER");
            b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("TableId");
            b.ToTable("Rows", (string)null);
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Table", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<string>("Name").IsRequired().HasMaxLength(256).HasColumnType("TEXT");
            b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
            b.HasKey("Id");
            b.ToTable("Tables", (string)null);
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.CellValue", b =>
        {
            b.HasOne("CustomizableTablesWithDeadlines.Domain.Entities.Column", "Column")
                .WithMany("CellValues")
                .HasForeignKey("ColumnId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.HasOne("CustomizableTablesWithDeadlines.Domain.Entities.Row", "Row")
                .WithMany("CellValues")
                .HasForeignKey("RowId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("Column");
            b.Navigation("Row");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Column", b =>
        {
            b.HasOne("CustomizableTablesWithDeadlines.Domain.Entities.Table", "Table")
                .WithMany("Columns")
                .HasForeignKey("TableId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("Table");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Deadline", b =>
        {
            b.HasOne("CustomizableTablesWithDeadlines.Domain.Entities.Row", "Row")
                .WithMany("Deadlines")
                .HasForeignKey("RowId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("Row");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.NotificationLog", b =>
        {
            b.HasOne("CustomizableTablesWithDeadlines.Domain.Entities.Deadline", "Deadline")
                .WithMany("NotificationLogs")
                .HasForeignKey("DeadlineId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.HasOne("CustomizableTablesWithDeadlines.Domain.Entities.NotificationRule", "NotificationRule")
                .WithMany("NotificationLogs")
                .HasForeignKey("NotificationRuleId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("Deadline");
            b.Navigation("NotificationRule");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.NotificationRule", b =>
        {
            b.HasOne("CustomizableTablesWithDeadlines.Domain.Entities.Deadline", "Deadline")
                .WithMany("NotificationRules")
                .HasForeignKey("DeadlineId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("Deadline");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Row", b =>
        {
            b.HasOne("CustomizableTablesWithDeadlines.Domain.Entities.Table", "Table")
                .WithMany("Rows")
                .HasForeignKey("TableId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("Table");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Column", b =>
        {
            b.Navigation("CellValues");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Deadline", b =>
        {
            b.Navigation("NotificationLogs");
            b.Navigation("NotificationRules");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.NotificationRule", b =>
        {
            b.Navigation("NotificationLogs");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Row", b =>
        {
            b.Navigation("CellValues");
            b.Navigation("Deadlines");
        });

        modelBuilder.Entity("CustomizableTablesWithDeadlines.Domain.Entities.Table", b =>
        {
            b.Navigation("Columns");
            b.Navigation("Rows");
        });
#pragma warning restore 612, 618
    }
}
