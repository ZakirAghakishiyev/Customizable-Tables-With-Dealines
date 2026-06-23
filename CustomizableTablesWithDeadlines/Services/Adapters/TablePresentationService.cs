using CustomizableTablesWithDeadlines.Application.DTOs.Columns;
using CustomizableTablesWithDeadlines.Application.DTOs.Rows;
using CustomizableTablesWithDeadlines.Application.DTOs.Tables;
using CustomizableTablesWithDeadlines.Mapping;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using AppColumnService = CustomizableTablesWithDeadlines.Application.Abstractions.Services.IColumnService;
using AppRowService = CustomizableTablesWithDeadlines.Application.Abstractions.Services.IRowService;
using AppTableService = CustomizableTablesWithDeadlines.Application.Abstractions.Services.ITableService;

namespace CustomizableTablesWithDeadlines.Services.Adapters;

public class TablePresentationService : ITableService
{
    private readonly AppTableService _tables;
    private readonly AppColumnService _columns;
    private readonly AppRowService _rows;

    public TablePresentationService(
        AppTableService tables,
        AppColumnService columns,
        AppRowService rows)
    {
        _tables = tables;
        _columns = columns;
        _rows = rows;
    }

    public event EventHandler? TablesChanged;

    public async Task<IReadOnlyList<CustomTable>> GetAllTablesAsync()
    {
        var tables = await _tables.GetAllAsync();
        return tables.Select(PresentationMapping.ToCustomTable).ToList();
    }

    public async Task<CustomTable?> GetTableByIdAsync(int id)
    {
        var table = await _tables.GetByIdAsync(id);
        return PresentationMapping.ToCustomTable(table);
    }

    public async Task<CustomTable> CreateTableAsync(string name)
    {
        var id = await _tables.CreateAsync(new CreateTableDto { Name = name });
        var created = new CustomTable
        {
            Id = id,
            Name = name.Trim(),
            LastUpdated = DateTime.Now
        };
        TablesChanged?.Invoke(this, EventArgs.Empty);
        return created;
    }

    public async Task<CustomTable> ImportTableAsync(CustomTable table)
    {
        TablesChanged?.Invoke(this, EventArgs.Empty);
        return (await GetTableByIdAsync(table.Id))!;
    }

    public async Task UpdateTableAsync(CustomTable table)
    {
        var current = await _tables.GetByIdAsync(table.Id);

        if (!string.Equals(current.Name, table.Name, StringComparison.Ordinal))
            await _tables.RenameAsync(new RenameTableDto { Id = table.Id, Name = table.Name });

        var currentColumnIds = current.Columns.Select(c => c.Id).ToHashSet();

        foreach (var column in table.Columns.Where(c => c.Id <= 0).ToList())
        {
            var oldColumnId = column.Id;
            var newColumnId = await _columns.CreateAsync(new CreateColumnDto
            {
                TableId = table.Id,
                Name = column.Name,
                DataType = PresentationMapping.ToColumnDataType(column.Type)
            });
            column.Id = newColumnId;
            RemapColumnCellValues(table, oldColumnId, newColumnId);
        }

        var uiColumnIds = table.Columns.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();

        foreach (var removedId in currentColumnIds.Except(uiColumnIds))
            await _columns.DeleteAsync(removedId);

        foreach (var column in table.Columns.Where(c => c.Id > 0 && currentColumnIds.Contains(c.Id)))
        {
            var existing = current.Columns.First(c => c.Id == column.Id);
            if (!string.Equals(existing.Name, column.Name, StringComparison.Ordinal))
            {
                await _columns.RenameAsync(new RenameColumnDto { Id = column.Id, Name = column.Name });
            }
        }

        var columnsToReorder = table.Columns
            .Where(c => c.Id > 0)
            .OrderBy(c => c.Order)
            .Select((c, index) => new ReorderColumnDto { Id = c.Id, OrderIndex = index })
            .ToList();

        if (columnsToReorder.Count > 0)
            await _columns.ReorderAsync(columnsToReorder);

        current = await _tables.GetByIdAsync(table.Id);
        var currentRowIds = current.Rows.Select(r => r.Id).ToHashSet();

        foreach (var uiRow in table.Rows.Where(r => r.Id <= 0).ToList())
        {
            var newRowId = await _rows.CreateAsync(new CreateRowDto { TableId = table.Id });
            uiRow.Id = newRowId;
        }

        var uiRowIds = table.Rows.Select(r => r.Id).ToHashSet();

        foreach (var removedRowId in currentRowIds.Except(uiRowIds))
            await _rows.DeleteAsync(removedRowId);

        current = await _tables.GetByIdAsync(table.Id);
        foreach (var uiRow in table.Rows)
        {
            var currentRow = current.Rows.FirstOrDefault(r => r.Id == uiRow.Id);
            if (currentRow is null)
                continue;

            foreach (var column in table.Columns)
            {
                if (column.Id <= 0)
                    continue;

                var cell = currentRow.Cells.FirstOrDefault(c => c.ColumnId == column.Id);
                uiRow.CellValues.TryGetValue(column.Id, out var uiValue);
                var dto = BuildCellUpdate(uiRow.Id, column.Id, column.Type, uiValue);

                if (cell is null || CellChanged(cell, column.Type, uiValue))
                    await _rows.UpdateCellValueAsync(dto);
            }
        }

        TablesChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task DeleteTableAsync(int id)
    {
        await _tables.DeleteAsync(id);
        TablesChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task RenameTableAsync(int id, string newName)
    {
        await _tables.RenameAsync(new RenameTableDto { Id = id, Name = newName });
        TablesChanged?.Invoke(this, EventArgs.Empty);
    }

    private static void RemapColumnCellValues(CustomTable table, int oldColumnId, int newColumnId)
    {
        if (oldColumnId == newColumnId)
            return;

        foreach (var row in table.Rows)
        {
            if (row.CellValues.Remove(oldColumnId, out var value))
                row.CellValues[newColumnId] = value;
        }
    }

    private static UpdateCellValueDto BuildCellUpdate(int rowId, int columnId, Models.Enums.ColumnType type, object? value) =>
        type switch
        {
            Models.Enums.ColumnType.Number => new UpdateCellValueDto
            {
                RowId = rowId,
                ColumnId = columnId,
                NumberValue = value is null ? null : Convert.ToDecimal(value)
            },
            Models.Enums.ColumnType.DateTime => new UpdateCellValueDto
            {
                RowId = rowId,
                ColumnId = columnId,
                DateTimeValue = value as DateTime?
            },
            Models.Enums.ColumnType.Boolean => new UpdateCellValueDto
            {
                RowId = rowId,
                ColumnId = columnId,
                BooleanValue = value as bool?
            },
            _ => new UpdateCellValueDto
            {
                RowId = rowId,
                ColumnId = columnId,
                TextValue = value?.ToString()
            }
        };

    private static bool CellChanged(
        Application.DTOs.Cells.CellValueDto cell,
        Models.Enums.ColumnType type,
        object? uiValue) => type switch
    {
        Models.Enums.ColumnType.Number => cell.NumberValue != (uiValue is null ? null : Convert.ToDecimal(uiValue)),
        Models.Enums.ColumnType.DateTime => cell.DateTimeValue != uiValue as DateTime?,
        Models.Enums.ColumnType.Boolean => cell.BooleanValue != uiValue as bool?,
        _ => !string.Equals(cell.TextValue, uiValue?.ToString(), StringComparison.Ordinal)
    };
}
