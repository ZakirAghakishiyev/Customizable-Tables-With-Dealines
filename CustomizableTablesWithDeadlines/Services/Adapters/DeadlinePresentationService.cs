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
        var tables = await _tables.GetAllAsync();
        var tableLookup = tables.ToDictionary(t => t.Id, t => t.Name);
        var rowContext = new Dictionary<int, (int TableId, string RowInfo)>();

        foreach (var tableId in deadlines.Select(d => d.RowId).Distinct())
        {
            foreach (var table in tables)
            {
                var details = await _tables.GetByIdAsync(table.Id);
                foreach (var row in details.Rows)
                {
                    rowContext[row.Id] = (table.Id, BuildRowInfo(row));
                }
            }
        }

        return deadlines.Select(d =>
        {
            rowContext.TryGetValue(d.RowId, out var context);
            return new DeadlineListItem
            {
                DeadlineId = d.Id,
                RowId = d.RowId,
                TableId = context.TableId,
                TableName = tableLookup.GetValueOrDefault(context.TableId, string.Empty),
                RowInfo = context.RowInfo,
                Title = d.Title,
                DeadlineDateTime = d.DeadlineDateTime
            };
        }).ToList();
    }

    private static string BuildRowInfo(Application.DTOs.Rows.RowDetailsDto row)
    {
        var first = row.Cells.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.TextValue));
        return first?.TextValue ?? $"Row {row.OrderNumber}";
    }
}
