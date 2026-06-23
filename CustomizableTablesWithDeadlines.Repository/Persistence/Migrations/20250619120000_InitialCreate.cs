using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Tables",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tables", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Columns",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                TableId = table.Column<int>(type: "INTEGER", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                DataType = table.Column<int>(type: "INTEGER", nullable: false),
                OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Columns", x => x.Id);
                table.ForeignKey(
                    name: "FK_Columns_Tables_TableId",
                    column: x => x.TableId,
                    principalTable: "Tables",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Rows",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                TableId = table.Column<int>(type: "INTEGER", nullable: false),
                OrderNumber = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Rows", x => x.Id);
                table.ForeignKey(
                    name: "FK_Rows_Tables_TableId",
                    column: x => x.TableId,
                    principalTable: "Tables",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Deadlines",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                RowId = table.Column<int>(type: "INTEGER", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                DeadlineDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Deadlines", x => x.Id);
                table.ForeignKey(
                    name: "FK_Deadlines_Rows_RowId",
                    column: x => x.RowId,
                    principalTable: "Rows",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CellValues",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                RowId = table.Column<int>(type: "INTEGER", nullable: false),
                ColumnId = table.Column<int>(type: "INTEGER", nullable: false),
                ValueText = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                ValueDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                ValueNumber = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                ValueBoolean = table.Column<bool>(type: "INTEGER", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CellValues", x => x.Id);
                table.ForeignKey(
                    name: "FK_CellValues_Columns_ColumnId",
                    column: x => x.ColumnId,
                    principalTable: "Columns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CellValues_Rows_RowId",
                    column: x => x.RowId,
                    principalTable: "Rows",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "NotificationRules",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                DeadlineId = table.Column<int>(type: "INTEGER", nullable: false),
                NotifyBeforeMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationRules", x => x.Id);
                table.ForeignKey(
                    name: "FK_NotificationRules_Deadlines_DeadlineId",
                    column: x => x.DeadlineId,
                    principalTable: "Deadlines",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "NotificationLogs",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                DeadlineId = table.Column<int>(type: "INTEGER", nullable: false),
                NotificationRuleId = table.Column<int>(type: "INTEGER", nullable: false),
                ScheduledFor = table.Column<DateTime>(type: "TEXT", nullable: false),
                SentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                Status = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationLogs", x => x.Id);
                table.ForeignKey(
                    name: "FK_NotificationLogs_Deadlines_DeadlineId",
                    column: x => x.DeadlineId,
                    principalTable: "Deadlines",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_NotificationLogs_NotificationRules_NotificationRuleId",
                    column: x => x.NotificationRuleId,
                    principalTable: "NotificationRules",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CellValues_ColumnId",
            table: "CellValues",
            column: "ColumnId");

        migrationBuilder.CreateIndex(
            name: "IX_CellValues_RowId_ColumnId",
            table: "CellValues",
            columns: new[] { "RowId", "ColumnId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Columns_TableId",
            table: "Columns",
            column: "TableId");

        migrationBuilder.CreateIndex(
            name: "IX_Deadlines_DeadlineDateTime",
            table: "Deadlines",
            column: "DeadlineDateTime");

        migrationBuilder.CreateIndex(
            name: "IX_Deadlines_RowId",
            table: "Deadlines",
            column: "RowId");

        migrationBuilder.CreateIndex(
            name: "IX_NotificationLogs_DeadlineId",
            table: "NotificationLogs",
            column: "DeadlineId");

        migrationBuilder.CreateIndex(
            name: "IX_NotificationLogs_NotificationRuleId",
            table: "NotificationLogs",
            column: "NotificationRuleId");

        migrationBuilder.CreateIndex(
            name: "IX_NotificationLogs_ScheduledFor",
            table: "NotificationLogs",
            column: "ScheduledFor");

        migrationBuilder.CreateIndex(
            name: "IX_NotificationLogs_Status",
            table: "NotificationLogs",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_NotificationRules_DeadlineId",
            table: "NotificationRules",
            column: "DeadlineId");

        migrationBuilder.CreateIndex(
            name: "IX_Rows_TableId",
            table: "Rows",
            column: "TableId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CellValues");
        migrationBuilder.DropTable(name: "NotificationLogs");
        migrationBuilder.DropTable(name: "NotificationRules");
        migrationBuilder.DropTable(name: "Deadlines");
        migrationBuilder.DropTable(name: "Columns");
        migrationBuilder.DropTable(name: "Rows");
        migrationBuilder.DropTable(name: "Tables");
    }
}
