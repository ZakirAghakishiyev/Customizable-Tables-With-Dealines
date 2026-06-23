using CustomizableTablesWithDeadlines.Application.Abstractions.Infrastructure;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CustomizableTablesWithDeadlines.Infrastructure;

public static class InfrastructureBootstrap
{
    public static async Task InitializeApplicationAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var databaseInitializer = serviceProvider.GetRequiredService<IDatabaseInitializer>();
        await databaseInitializer.InitializeAsync(cancellationToken);

        var notificationScheduler = serviceProvider.GetRequiredService<INotificationScheduler>();
        await notificationScheduler.ScheduleAllFutureNotificationsAsync(cancellationToken);
    }
}
