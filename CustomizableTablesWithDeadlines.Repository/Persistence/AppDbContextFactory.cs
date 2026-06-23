using CustomizableTablesWithDeadlines.Application.Abstractions.Infrastructure;
using CustomizableTablesWithDeadlines.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var pathProvider = new Paths.PathProvider();
        Directory.CreateDirectory(pathProvider.GetAppDataDirectory());

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={pathProvider.GetDatabasePath()}");
        return new AppDbContext(optionsBuilder.Options);
    }
}
