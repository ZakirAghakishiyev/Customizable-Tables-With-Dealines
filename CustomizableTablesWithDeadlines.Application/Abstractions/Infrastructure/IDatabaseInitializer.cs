namespace CustomizableTablesWithDeadlines.Application.Abstractions.Infrastructure;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
