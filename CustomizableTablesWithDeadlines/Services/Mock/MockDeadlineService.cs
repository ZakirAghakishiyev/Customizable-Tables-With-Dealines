using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.Services.Mock;

public class MockDeadlineService : IDeadlineService
{
    private readonly ITableService _tableService;

    public MockDeadlineService(ITableService tableService)
    {
        _tableService = tableService;
        _tableService.TablesChanged += (_, _) => { };
    }

    public async Task<IReadOnlyList<DeadlineListItem>> GetAllDeadlinesAsync()
    {
        var tables = await _tableService.GetAllTablesAsync();
        return FlattenDeadlines(tables);
    }

    public async Task<IReadOnlyList<DeadlineListItem>> GetUpcomingDeadlinesAsync(int count)
    {
        var all = await GetAllDeadlinesAsync();
        return all
            .Where(d => !d.IsCompleted && d.DeadlineDateTime >= DateTime.Now)
            .OrderBy(d => d.DeadlineDateTime)
            .Take(count)
            .ToList();
    }

    public async Task<int> GetUpcomingCountAsync()
    {
        var all = await GetAllDeadlinesAsync();
        return all.Count(d => !d.IsCompleted && d.DeadlineDateTime >= DateTime.Now);
    }

    public async Task<int> GetOverdueCountAsync()
    {
        var all = await GetAllDeadlinesAsync();
        return all.Count(d => !d.IsCompleted && d.DeadlineDateTime < DateTime.Now);
    }

    public DeadlineFilter ApplyFilter(IEnumerable<DeadlineListItem> deadlines, DeadlineFilter filter)
    {
        return filter;
    }

    public static IEnumerable<DeadlineListItem> FilterDeadlines(
        IEnumerable<DeadlineListItem> deadlines,
        DeadlineFilter filter)
    {
        var today = DateTime.Today;
        var weekEnd = today.AddDays(7);

        return filter switch
        {
            DeadlineFilter.Today => deadlines.Where(d =>
                d.DeadlineDateTime.Date == today),
            DeadlineFilter.ThisWeek => deadlines.Where(d =>
                d.DeadlineDateTime.Date >= today && d.DeadlineDateTime.Date < weekEnd),
            DeadlineFilter.Overdue => deadlines.Where(d =>
                !d.IsCompleted && d.DeadlineDateTime < DateTime.Now),
            _ => deadlines
        };
    }

    private static List<DeadlineListItem> FlattenDeadlines(IReadOnlyList<CustomTable> tables)
    {
        var result = new List<DeadlineListItem>();

        foreach (var table in tables)
        {
            foreach (var row in table.Rows)
            {
                var rowInfo = GetRowInfo(table, row);
                foreach (var deadline in row.Deadlines)
                {
                    result.Add(new DeadlineListItem
                    {
                        DeadlineId = deadline.Id,
                        TableId = table.Id,
                        RowId = row.Id,
                        TableName = table.Name,
                        RowInfo = rowInfo,
                        Title = deadline.Title,
                        DeadlineDateTime = deadline.DeadlineDateTime,
                        IsCompleted = deadline.IsCompleted
                    });
                }
            }
        }

        return result.OrderBy(d => d.DeadlineDateTime).ToList();
    }

    private static string GetRowInfo(CustomTable table, TableRowData row)
    {
        var firstCol = table.Columns.OrderBy(c => c.Order).FirstOrDefault();
        if (firstCol is null)
            return row.Id.ToString()[..8];

        return row.CellValues.TryGetValue(firstCol.Id, out var value) && value is not null
            ? value.ToString() ?? "—"
            : "—";
    }
}
