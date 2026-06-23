using CustomizableTablesWithDeadlines.Application.Abstractions.Infrastructure;
using CustomizableTablesWithDeadlines.Domain.Entities;
using CustomizableTablesWithDeadlines.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly AppDbContext _context;
    private readonly IPathProvider _pathProvider;
    private readonly IConfiguration _configuration;

    public DatabaseInitializer(AppDbContext context, IPathProvider pathProvider, IConfiguration configuration)
    {
        _context = context;
        _pathProvider = pathProvider;
        _configuration = configuration;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var directory = _pathProvider.GetAppDataDirectory();
        Directory.CreateDirectory(directory);

        await _context.Database.MigrateAsync(cancellationToken);

        if (IsDevelopmentMode() && !await _context.Tables.AnyAsync(cancellationToken))
            await SeedSampleDataAsync(cancellationToken);
    }

    private bool IsDevelopmentMode()
    {
        var configured = _configuration["Database:SeedSampleData"];
        if (bool.TryParse(configured, out var seed))
            return seed;

        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase);
    }

    private async Task SeedSampleDataAsync(CancellationToken cancellationToken)
    {
        var table = new Table { Name = "Project Tasks" };
        var colTask = new Column { Name = "Task", DataType = ColumnDataType.Text, OrderIndex = 0 };
        var colDue = new Column { Name = "Due Date", DataType = ColumnDataType.DateTime, OrderIndex = 1 };
        table.Columns.Add(colTask);
        table.Columns.Add(colDue);

        var row = new Row { OrderNumber = 1 };
        row.CellValues.Add(new CellValue
        {
            Column = colTask,
            ValueText = "Design mockups"
        });
        row.CellValues.Add(new CellValue
        {
            Column = colDue,
            ValueDateTime = DateTime.UtcNow.AddDays(3)
        });

        var deadline = new Deadline
        {
            Title = "Submit designs",
            DeadlineDateTime = DateTime.UtcNow.AddDays(2)
        };
        deadline.NotificationRules.Add(new NotificationRule
        {
            NotifyBeforeMinutes = 1440,
            IsEnabled = true
        });
        row.Deadlines.Add(deadline);
        table.Rows.Add(row);

        await _context.Tables.AddAsync(table, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
