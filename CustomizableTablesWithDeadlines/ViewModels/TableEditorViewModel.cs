using System.Collections.ObjectModel;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using CustomizableTablesWithDeadlines.Views.Dialogs;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class TableEditorViewModel : LocalizedViewModelBase
{
    private readonly ITableService _tableService;
    private readonly INavigationService _navigationService;
    private readonly Func<TableRowData, DeadlineManagementViewModel> _deadlineVmFactory;
    private CustomTable? _table;
    private DataTable _dataTable = new();

    [ObservableProperty] private string _tableName = string.Empty;
    [ObservableProperty] private ObservableCollection<TableColumnDefinition> _columns = [];
    [ObservableProperty] private TableColumnDefinition? _selectedColumn;
    [ObservableProperty] private ColumnType _selectedColumnType = ColumnType.Text;
    [ObservableProperty] private DataView? _gridData;

    public int TableId { get; private set; }

    public TableEditorViewModel(
        ILocalizationService localization,
        ITableService tableService,
        INavigationService navigationService,
        Func<TableRowData, DeadlineManagementViewModel> deadlineVmFactory) : base(localization)
    {
        _tableService = tableService;
        _navigationService = navigationService;
        _deadlineVmFactory = deadlineVmFactory;
    }

    public event Action? GridStructureChanged;

    public async Task InitializeAsync(int tableId)
    {
        TableId = tableId;
        _table = await _tableService.GetTableByIdAsync(tableId);
        if (_table is null)
            return;

        TableName = _table.Name;
        Columns = new ObservableCollection<TableColumnDefinition>(_table.Columns.OrderBy(c => c.Order));
        RebuildGrid();
    }

    [RelayCommand]
    private void BackToTables()
    {
        _navigationService.NavigateTo<TablesViewModel>();
    }

    [RelayCommand]
    private void AddColumn()
    {
        if (_table is null) return;

        var order = _table.Columns.Count;
        var column = new TableColumnDefinition
        {
            Name = $"Column {order + 1}",
            Type = SelectedColumnType,
            Order = order
        };
        _table.Columns.Add(column);
        Columns.Add(column);
        RebuildGrid();
    }

    [RelayCommand]
    private void RenameColumn()
    {
        if (SelectedColumn is null) return;

        var dialog = new InputDialog(Strings.RenameColumn, Strings.EnterName, SelectedColumn.Name);
        if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.InputText))
        {
            SelectedColumn.Name = dialog.InputText.Trim();
            OnPropertyChanged(nameof(Columns));
            RebuildGrid();
        }
    }

    [RelayCommand]
    private void DeleteColumn()
    {
        if (SelectedColumn is null || _table is null) return;

        if (!MessageBoxHelper.Confirm(Strings.ConfirmDelete))
            return;

        var colId = SelectedColumn.Id;
        _table.Columns.RemoveAll(c => c.Id == colId);
        foreach (var row in _table.Rows)
            row.CellValues.Remove(colId);

        Columns.Remove(SelectedColumn);
        ReorderColumns();
        SelectedColumn = null;
        RebuildGrid();
    }

    [RelayCommand]
    private void MoveColumnLeft()
    {
        if (SelectedColumn is null) return;
        MoveColumn(SelectedColumn, -1);
    }

    [RelayCommand]
    private void MoveColumnRight()
    {
        if (SelectedColumn is null) return;
        MoveColumn(SelectedColumn, 1);
    }

    [RelayCommand]
    private void ChangeColumnType()
    {
        if (SelectedColumn is null) return;
        SelectedColumn.Type = SelectedColumnType;
        RebuildGrid();
    }

    [RelayCommand]
    private void AddRow()
    {
        if (_table is null) return;

        var row = new TableRowData();
        foreach (var col in _table.Columns)
            row.CellValues[col.Id] = GetDefaultValue(col.Type);

        _table.Rows.Add(row);
        RebuildGrid();
    }

    [RelayCommand]
    private void DeleteRow(int rowId)
    {
        if (_table is null) return;
        if (!MessageBoxHelper.Confirm(Strings.ConfirmDelete))
            return;

        _table.Rows.RemoveAll(r => r.Id == rowId);
        RebuildGrid();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (_table is null) return;

        SyncGridToModel();
        _table.Name = TableName;
        await _tableService.UpdateTableAsync(_table);
    }

    [RelayCommand]
    private void ManageDeadlines(int rowId)
    {
        if (_table is null) return;

        var row = _table.Rows.FirstOrDefault(r => r.Id == rowId);
        if (row is null) return;

        var vm = _deadlineVmFactory(row);
        var dialog = new DeadlineManagementDialog(vm);
        dialog.ShowDialog();
        RebuildGrid();
    }

    public void RebuildGrid()
    {
        if (_table is null) return;

        _dataTable = new DataTable();
        _dataTable.Columns.Add("_RowId", typeof(int));

        foreach (var col in _table.Columns.OrderBy(c => c.Order))
            _dataTable.Columns.Add(col.Id.ToString(), GetClrType(col.Type));

        foreach (var row in _table.Rows)
        {
            var dr = _dataTable.NewRow();
            dr["_RowId"] = row.Id;
            foreach (var col in _table.Columns)
            {
                var value = row.CellValues.GetValueOrDefault(col.Id);
                dr[col.Id.ToString()] = value ?? DBNull.Value;
            }
            _dataTable.Rows.Add(dr);
        }

        GridData = _dataTable.DefaultView;
        GridStructureChanged?.Invoke();
    }

    private void SyncGridToModel()
    {
        if (_table is null) return;

        _dataTable.AcceptChanges();
        foreach (DataRow dr in _dataTable.Rows)
        {
            var rowId = (int)dr["_RowId"];
            var row = _table.Rows.FirstOrDefault(r => r.Id == rowId);
            if (row is null) continue;

            foreach (var col in _table.Columns)
            {
                var val = dr[col.Id.ToString()];
                row.CellValues[col.Id] = val == DBNull.Value ? null : val;
            }
        }
    }

    private void MoveColumn(TableColumnDefinition column, int direction)
    {
        if (_table is null) return;

        var ordered = _table.Columns.OrderBy(c => c.Order).ToList();
        var index = ordered.FindIndex(c => c.Id == column.Id);
        var newIndex = index + direction;
        if (newIndex < 0 || newIndex >= ordered.Count)
            return;

        (ordered[index].Order, ordered[newIndex].Order) = (ordered[newIndex].Order, ordered[index].Order);
        Columns = new ObservableCollection<TableColumnDefinition>(ordered.OrderBy(c => c.Order));
        RebuildGrid();
    }

    private void ReorderColumns()
    {
        if (_table is null) return;

        var ordered = _table.Columns.OrderBy(c => c.Order).ToList();
        for (var i = 0; i < ordered.Count; i++)
            ordered[i].Order = i;
        Columns = new ObservableCollection<TableColumnDefinition>(ordered);
    }

    private static object? GetDefaultValue(ColumnType type) => type switch
    {
        ColumnType.Number => 0,
        ColumnType.DateTime => DateTime.Now,
        ColumnType.Boolean => false,
        _ => string.Empty
    };

    private static Type GetClrType(ColumnType type) => type switch
    {
        ColumnType.Number => typeof(double),
        ColumnType.DateTime => typeof(DateTime),
        ColumnType.Boolean => typeof(bool),
        _ => typeof(string)
    };
}
