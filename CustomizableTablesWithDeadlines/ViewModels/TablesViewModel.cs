using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using CustomizableTablesWithDeadlines.Views.Dialogs;
using Microsoft.Win32;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class TablesViewModel : LocalizedViewModelBase
{
    private readonly ITableService _tableService;
    private readonly IWordImportService _wordImportService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private ObservableCollection<CustomTable> _tables = [];
    [ObservableProperty] private CustomTable? _selectedTable;

    public TablesViewModel(
        ILocalizationService localization,
        ITableService tableService,
        IWordImportService wordImportService,
        INavigationService navigationService) : base(localization)
    {
        _tableService = tableService;
        _wordImportService = wordImportService;
        _navigationService = navigationService;
        _tableService.TablesChanged += async (_, _) => await LoadAsync();
    }

    public async Task LoadAsync()
    {
        var tables = await _tableService.GetAllTablesAsync();
        Tables = new ObservableCollection<CustomTable>(tables);
    }

    [RelayCommand]
    private async Task ImportFromWordAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = Strings.ImportFromWord,
            Filter = Strings.WordDocumentFilter,
            CheckFileExists = true
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            var tables = _wordImportService.ParseTables(dialog.FileName);
            if (tables.Count == 0)
            {
                MessageBox.Show(
                    Strings.NoTablesFoundInWord,
                    Strings.ImportFromWord,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var defaultName = Path.GetFileNameWithoutExtension(dialog.FileName);
            var previewVm = new ImportWordPreviewViewModel(
                App.GetService<ILocalizationService>(),
                _wordImportService,
                tables,
                defaultName);
            var previewDialog = new ImportWordPreviewDialog(previewVm)
            {
                Owner = Application.Current.MainWindow
            };

            if (previewDialog.ShowDialog() == true && previewDialog.Result is not null)
            {
                await _tableService.ImportTableAsync(previewDialog.Result);
                await LoadAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                string.Format(Strings.ImportWordFailed, ex.Message),
                Strings.ImportFromWord,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task CreateTableAsync()
    {
        var dialog = new InputDialog(Strings.CreateNewTable, Strings.EnterName, "New Table");
        if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.InputText))
        {
            await _tableService.CreateTableAsync(dialog.InputText.Trim());
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task RenameTableAsync()
    {
        if (SelectedTable is null)
            return;

        var dialog = new InputDialog(Strings.RenameTable, Strings.EnterName, SelectedTable.Name);
        if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.InputText))
        {
            await _tableService.RenameTableAsync(SelectedTable.Id, dialog.InputText.Trim());
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteTableAsync()
    {
        if (SelectedTable is null)
            return;

        if (MessageBoxHelper.Confirm(Strings.ConfirmDelete))
        {
            await _tableService.DeleteTableAsync(SelectedTable.Id);
            SelectedTable = null;
            await LoadAsync();
        }
    }

    [RelayCommand]
    private void OpenTable(CustomTable? table)
    {
        var target = table ?? SelectedTable;
        if (target is not null)
            _navigationService.NavigateToTableEditor(target.Id);
    }

    [RelayCommand]
    private async Task DeleteTableItemAsync(CustomTable table)
    {
        if (MessageBoxHelper.Confirm(Strings.ConfirmDelete))
        {
            await _tableService.DeleteTableAsync(table.Id);
            if (SelectedTable?.Id == table.Id)
                SelectedTable = null;
            await LoadAsync();
        }
    }
}
