using System.Collections.ObjectModel;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Services;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class ImportWordPreviewViewModel : LocalizedViewModelBase
{
    private readonly IWordImportService _wordImportService;
    private readonly IReadOnlyList<WordTablePreview> _tables;
    private DataTable _previewTable = new();

    [ObservableProperty] private WordTablePreview? _selectedTable;
    [ObservableProperty] private string _tableName = string.Empty;
    [ObservableProperty] private DataView? _previewData;
    [ObservableProperty] private ObservableCollection<DeadlineColumnSuggestion> _deadlineSuggestions = [];

    public CustomTable? Result { get; private set; }

    public ImportWordPreviewViewModel(
        ILocalizationService localization,
        IWordImportService wordImportService,
        IReadOnlyList<WordTablePreview> tables,
        string defaultTableName) : base(localization)
    {
        _wordImportService = wordImportService;
        _tables = tables;
        TableName = defaultTableName;
        SelectedTable = tables.FirstOrDefault();
    }

    public IReadOnlyList<WordTablePreview> Tables => _tables;

    partial void OnSelectedTableChanged(WordTablePreview? value)
    {
        if (value is null)
            return;

        RebuildPreview(value);
        UpdateDeadlineSuggestions(value);
    }

    [RelayCommand]
    private void Import()
    {
        if (SelectedTable is null || string.IsNullOrWhiteSpace(TableName))
            return;

        var deadlineIndices = DeadlineSuggestions
            .Where(s => s.IsSelected)
            .Select(s => s.ColumnIndex)
            .ToList();

        Result = _wordImportService.BuildTable(
            SelectedTable,
            TableName.Trim(),
            deadlineIndices);
    }

    private void RebuildPreview(WordTablePreview table)
    {
        _previewTable = new DataTable();
        if (table.Rows.Count == 0)
        {
            PreviewData = _previewTable.DefaultView;
            return;
        }

        var headerRow = table.Rows[0];
        var columnNames = new List<string>();
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < headerRow.Count; i++)
        {
            var name = string.IsNullOrWhiteSpace(headerRow[i]) ? $"Column {i + 1}" : headerRow[i].Trim();
            var candidate = name;
            var suffix = 2;
            while (!used.Add(candidate))
                candidate = $"{name} ({suffix++})";
            columnNames.Add(candidate);
            _previewTable.Columns.Add(candidate);
        }

        foreach (var row in table.Rows)
        {
            var dataRow = _previewTable.NewRow();
            for (var i = 0; i < columnNames.Count; i++)
                dataRow[i] = i < row.Count ? row[i] : string.Empty;
            _previewTable.Rows.Add(dataRow);
        }

        PreviewData = _previewTable.DefaultView;
    }

    private void UpdateDeadlineSuggestions(WordTablePreview table)
    {
        if (table.Rows.Count == 0)
        {
            DeadlineSuggestions = [];
            return;
        }

        var headerRow = table.Rows[0];
        var columnNames = headerRow
            .Select((name, i) => string.IsNullOrWhiteSpace(name) ? $"Column {i + 1}" : name.Trim())
            .ToList();
        var dataRows = table.Rows.Skip(1).ToList();
        var suggested = WordImportService.SuggestDeadlineColumns(columnNames, dataRows);

        DeadlineSuggestions = new ObservableCollection<DeadlineColumnSuggestion>(
            suggested.Select(i => new DeadlineColumnSuggestion
            {
                ColumnIndex = i,
                ColumnName = columnNames[i],
                IsSelected = true
            }));
    }
}
