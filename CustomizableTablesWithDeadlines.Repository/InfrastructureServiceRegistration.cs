using CustomizableTablesWithDeadlines.Application.Abstractions.Infrastructure;
using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Infrastructure.Notifications;
using CustomizableTablesWithDeadlines.Infrastructure.Persistence;
using CustomizableTablesWithDeadlines.Infrastructure.Persistence.Repositories;
using CustomizableTablesWithDeadlines.Infrastructure.Paths;
using CustomizableTablesWithDeadlines.Infrastructure.Scheduling;
using CustomizableTablesWithDeadlines.Infrastructure.Settings;
using CustomizableTablesWithDeadlines.Infrastructure.WordImport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace CustomizableTablesWithDeadlines.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPathProvider, PathProvider>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var pathProvider = serviceProvider.GetRequiredService<IPathProvider>();
            var connectionString = configuration.GetConnectionString("Default")
                                   ?? $"Data Source={pathProvider.GetDatabasePath()}";
            options.UseSqlite(connectionString);
        });

        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IDeadlineRepository, DeadlineRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IRepository<Domain.Entities.Column>, Repository<Domain.Entities.Column>>();
        services.AddScoped<IRepository<Domain.Entities.Row>, Repository<Domain.Entities.Row>>();
        services.AddScoped<IRepository<Domain.Entities.CellValue>, Repository<Domain.Entities.CellValue>>();
        services.AddScoped<IRepository<Domain.Entities.NotificationRule>, Repository<Domain.Entities.NotificationRule>>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IAppSettingsService, JsonAppSettingsService>();
        services.AddSingleton<IWordImportService, WordImportService>();

        services.AddQuartz(quartz =>
        {
            quartz.AddJob<DeadlineNotificationJob>(job => job.StoreDurably());
        });

        services.AddSingleton<INotificationScheduler, QuartzNotificationScheduler>();
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();

        return services;
    }
}
