using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.DTOs.Import;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using Microsoft.Win32;
using PresentationTableService = CustomizableTablesWithDeadlines.Services.Interfaces.ITableService;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class ImportViewModel : LocalizedViewModelBase
{
    private readonly IImportService _importService;
    private readonly PresentationTableService _tableService;

    [ObservableProperty] private ObservableCollection<DetectedWordTableDto> _detectedTables = [];
    [ObservableProperty] private DetectedWordTableDto? _selectedTable;
    [ObservableProperty] private string _tableName = string.Empty;
    [ObservableProperty] private bool _firstRowAsHeader = true;
    [ObservableProperty] private bool _createDeadlinesFromDateColumns;
    [ObservableProperty] private string? _selectedFilePath;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public ImportViewModel(
        ILocalizationService localization,
        IImportService importService,
        PresentationTableService tableService) : base(localization)
    {
        _importService = importService;
        _tableService = tableService;
    }

    [RelayCommand]
    private async Task BrowseFileAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = Strings.ImportFromWord,
            Filter = Strings.WordDocumentFilter,
            CheckFileExists = true
        };

        if (dialog.ShowDialog() != true)
            return;

        SelectedFilePath = dialog.FileName;
        TableName = Path.GetFileNameWithoutExtension(dialog.FileName);

        try
        {
            var tables = await _importService.DetectWordTablesAsync(dialog.FileName);
            DetectedTables = new ObservableCollection<DetectedWordTableDto>(tables);
            SelectedTable = DetectedTables.FirstOrDefault();
            StatusMessage = tables.Count == 0 ? Strings.NoTablesFoundInWord : string.Empty;
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format(Strings.ImportWordFailed, ex.Message);
            MessageBox.Show(StatusMessage, Strings.ImportFromWord, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task ImportSelectedTableAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedFilePath) || SelectedTable is null)
            return;

        try
        {
            var tableId = await _importService.ImportWordTableAsync(new ImportTableRequestDto
            {
                FilePath = SelectedFilePath,
                TableIndex = SelectedTable.TableIndex,
                FirstRowAsHeader = FirstRowAsHeader,
                TableName = string.IsNullOrWhiteSpace(TableName) ? null : TableName,
                CreateDeadlinesFromDateColumns = CreateDeadlinesFromDateColumns
            });

            StatusMessage = $"{Strings.Import} completed (ID: {tableId}).";
            await _tableService.GetAllTablesAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format(Strings.ImportWordFailed, ex.Message);
            MessageBox.Show(StatusMessage, Strings.ImportFromWord, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
