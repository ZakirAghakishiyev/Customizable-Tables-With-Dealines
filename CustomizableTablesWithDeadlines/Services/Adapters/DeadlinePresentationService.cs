using CustomizableTablesWithDeadlines.Mapping;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using AppDeadlineService = CustomizableTablesWithDeadlines.Application.Abstractions.Services.IDeadlineService;
using AppTableService = CustomizableTablesWithDeadlines.Application.Abstractions.Services.ITableService;

namespace CustomizableTablesWithDeadlines.Services.Adapters;

public class DeadlinePresentationService : IDeadlineService
{
    private readonly AppDeadlineService _deadlines;
    private readonly AppTableService _tables;

    public DeadlinePresentationService(AppDeadlineService deadlines, AppTableService tables)
    {
        _deadlines = deadlines;
        _tables = tables;
    }

    public async Task<IReadOnlyList<DeadlineListItem>> GetAllDeadlinesAsync()
    {
        var deadlines = await _deadlines.GetAllAsync();
        return await MapDeadlinesAsync(deadlines);
    }

    public async Task<IReadOnlyList<DeadlineListItem>> GetUpcomingDeadlinesAsync(int count)
    {
        var deadlines = await _deadlines.GetUpcomingAsync();
        var mapped = await MapDeadlinesAsync(deadlines);
        return mapped.Take(count).ToList();
    }

    public async Task<int> GetUpcomingCountAsync()
    {
        var deadlines = await _deadlines.GetUpcomingAsync();
        return deadlines.Count;
    }

    public async Task<int> GetOverdueCountAsync()
    {
        var deadlines = await _deadlines.GetOverdueAsync();
        return deadlines.Count;
    }

    public DeadlineFilter ApplyFilter(IEnumerable<DeadlineListItem> deadlines, DeadlineFilter filter) =>
        filter;

    private async Task<List<DeadlineListItem>> MapDeadlinesAsync(
        IReadOnlyList<Application.DTOs.Deadlines.DeadlineDto> deadlines)
    {
        if (deadlines.Count == 0)
            return [];

        var tables = await _tables.GetAllAsync();
        var tableLookup = tables.ToDictionary(t => t.Id, t => t.Name);
        var rowIds = deadlines.Select(d => d.RowId).ToHashSet();
        var rowContext = new Dictionary<int, (int TableId, string RowInfo)>();

        foreach (var table in tables)
        {
            try
            {
                var details = await _tables.GetByIdAsync(table.Id);
                foreach (var row in details.Rows.Where(r => rowIds.Contains(r.Id)))
                    rowContext[row.Id] = (table.Id, BuildRowInfo(row));
            }
            catch
            {
                // Skip tables that cannot be loaded; deadlines still appear with partial context.
            }
        }

        return deadlines
            .Select(d =>
            {
                rowContext.TryGetValue(d.RowId, out var context);
                return new DeadlineListItem
                {
                    DeadlineId = d.Id,
                    RowId = d.RowId,
                    TableId = context.TableId,
                    TableName = tableLookup.GetValueOrDefault(context.TableId, string.Empty),
                    RowInfo = context.RowInfo ?? string.Empty,
                    Title = d.Title,
                    DeadlineDateTime = d.DeadlineDateTime
                };
            })
            .OrderBy(d => d.DeadlineDateTime)
            .ToList();
    }

    private static string BuildRowInfo(Application.DTOs.Rows.RowDetailsDto row)
    {
        var first = row.Cells.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.TextValue));
        return first?.TextValue ?? $"Row {row.OrderNumber}";
    }
}
