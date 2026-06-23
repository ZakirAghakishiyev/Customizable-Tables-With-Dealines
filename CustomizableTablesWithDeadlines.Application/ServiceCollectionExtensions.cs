using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CustomizableTablesWithDeadlines.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITableService, TableService>();
        services.AddScoped<IColumnService, ColumnService>();
        services.AddScoped<IRowService, RowService>();
        services.AddScoped<IDeadlineService, DeadlineService>();
        services.AddScoped<INotificationRuleService, NotificationRuleService>();
        services.AddScoped<IImportService, ImportService>();
        services.AddScoped<ISettingsService, SettingsService>();

        return services;
    }
}
