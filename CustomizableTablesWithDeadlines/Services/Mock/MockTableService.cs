using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.Services.Mock;

public class MockTableService : ITableService
{
    private readonly List<CustomTable> _tables;

    public event EventHandler? TablesChanged;

    public MockTableService()
    {
        _tables = CreateSampleData();
    }

    public Task<IReadOnlyList<CustomTable>> GetAllTablesAsync() =>
        Task.FromResult<IReadOnlyList<CustomTable>>(_tables.ToList());

    public Task<CustomTable?> GetTableByIdAsync(Guid id) =>
        Task.FromResult(_tables.FirstOrDefault(t => t.Id == id));

    public Task<CustomTable> CreateTableAsync(string name)
    {
        var table = new CustomTable
        {
            Name = name,
            Columns =
            [
                new TableColumnDefinition { Name = "Name", Type = ColumnType.Text, Order = 0 },
                new TableColumnDefinition { Name = "Status", Type = ColumnType.Text, Order = 1 }
            ],
            Rows = [],
            LastUpdated = DateTime.Now
        };
        _tables.Add(table);
        RaiseChanged();
        return Task.FromResult(table);
    }

    public Task<CustomTable> ImportTableAsync(CustomTable table)
    {
        table.LastUpdated = DateTime.Now;
        _tables.Add(table);
        RaiseChanged();
        return Task.FromResult(table);
    }

    public Task UpdateTableAsync(CustomTable table)
    {
        var index = _tables.FindIndex(t => t.Id == table.Id);
        if (index >= 0)
        {
            table.LastUpdated = DateTime.Now;
            _tables[index] = table;
            RaiseChanged();
        }
        return Task.CompletedTask;
    }

    public Task DeleteTableAsync(Guid id)
    {
        _tables.RemoveAll(t => t.Id == id);
        RaiseChanged();
        return Task.CompletedTask;
    }

    public Task RenameTableAsync(Guid id, string newName)
    {
        var table = _tables.FirstOrDefault(t => t.Id == id);
        if (table is not null)
        {
            table.Name = newName;
            table.LastUpdated = DateTime.Now;
            RaiseChanged();
        }
        return Task.CompletedTask;
    }

    private void RaiseChanged() => TablesChanged?.Invoke(this, EventArgs.Empty);

    private static List<CustomTable> CreateSampleData()
    {
        var projectTable = new CustomTable
        {
            Name = "Project Tasks",
            LastUpdated = DateTime.Now.AddDays(-2),
            Columns =
            [
                new TableColumnDefinition { Name = "Task", Type = ColumnType.Text, Order = 0 },
                new TableColumnDefinition { Name = "Priority", Type = ColumnType.Number, Order = 1 },
                new TableColumnDefinition { Name = "Due Date", Type = ColumnType.DateTime, Order = 2 },
                new TableColumnDefinition { Name = "Done", Type = ColumnType.Boolean, Order = 3 }
            ]
        };

        var colTask = projectTable.Columns[0].Id;
        var colPriority = projectTable.Columns[1].Id;
        var colDue = projectTable.Columns[2].Id;
        var colDone = projectTable.Columns[3].Id;

        var row1 = new TableRowData
        {
            CellValues = new Dictionary<Guid, object?>
            {
                [colTask] = "Design mockups",
                [colPriority] = 1,
                [colDue] = DateTime.Now.AddDays(3),
                [colDone] = false
            },
            Deadlines =
            [
                new DeadlineItem
                {
                    Title = "Submit designs",
                    DeadlineDateTime = DateTime.Now.AddDays(2),
                    NotifyBefore = NotifyBeforeOption.OneDay
                }
            ]
        };

        var row2 = new TableRowData
        {
            CellValues = new Dictionary<Guid, object?>
            {
                [colTask] = "Implement API",
                [colPriority] = 2,
                [colDue] = DateTime.Now.AddDays(-1),
                [colDone] = false
            },
            Deadlines =
            [
                new DeadlineItem
                {
                    Title = "API deadline",
                    DeadlineDateTime = DateTime.Now.AddHours(-5),
                    NotifyBefore = NotifyBeforeOption.OneHour
                }
            ]
        };

        projectTable.Rows = [row1, row2];

        var inventoryTable = new CustomTable
        {
            Name = "Inventory",
            LastUpdated = DateTime.Now.AddDays(-5),
            Columns =
            [
                new TableColumnDefinition { Name = "Item", Type = ColumnType.Text, Order = 0 },
                new TableColumnDefinition { Name = "Quantity", Type = ColumnType.Number, Order = 1 }
            ]
        };

        var colItem = inventoryTable.Columns[0].Id;
        var colQty = inventoryTable.Columns[1].Id;

        inventoryTable.Rows =
        [
            new TableRowData
            {
                CellValues = new Dictionary<Guid, object?>
                {
                    [colItem] = "Laptop",
                    [colQty] = 12
                },
                Deadlines =
                [
                    new DeadlineItem
                    {
                        Title = "Restock check",
                        DeadlineDateTime = DateTime.Now.AddHours(6),
                        NotifyBefore = NotifyBeforeOption.ThirtyMinutes
                    }
                ]
            }
        ];

        return [projectTable, inventoryTable];
    }
}
