using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using CustomizableTablesWithDeadlines.Views.Dialogs;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class TablesViewModel : LocalizedViewModelBase
{
    private readonly ITableService _tableService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private ObservableCollection<CustomTable> _tables = [];
    [ObservableProperty] private CustomTable? _selectedTable;

    public TablesViewModel(
        ILocalizationService localization,
        ITableService tableService,
        INavigationService navigationService) : base(localization)
    {
        _tableService = tableService;
        _navigationService = navigationService;
        _tableService.TablesChanged += async (_, _) => await LoadAsync();
    }

    public async Task LoadAsync()
    {
        var tables = await _tableService.GetAllTablesAsync();
        Tables = new ObservableCollection<CustomTable>(tables);
    }

    [RelayCommand]
    private void ImportFromWord() => _navigationService.NavigateTo<ImportViewModel>();

    [RelayCommand]
    private async Task CreateTableAsync()
    {
        var dialog = new InputDialog(Strings.CreateNewTable, Strings.TableName)
        {
            Owner = System.Windows.Application.Current.MainWindow
        };

        if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.InputText))
            return;

        await _tableService.CreateTableAsync(dialog.InputText.Trim());
        await LoadAsync();
    }

    [RelayCommand]
    private void OpenTable(CustomTable? table)
    {
        if (table is null) return;
        _navigationService.NavigateToTableEditor(table.Id);
    }

    [RelayCommand]
    private async Task RenameTableAsync()
    {
        if (SelectedTable is null) return;

        var dialog = new InputDialog(Strings.RenameTable, Strings.TableName, SelectedTable.Name)
        {
            Owner = System.Windows.Application.Current.MainWindow
        };

        if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.InputText))
            return;

        SelectedTable.Name = dialog.InputText.Trim();
        await _tableService.UpdateTableAsync(SelectedTable);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteTableAsync()
    {
        if (SelectedTable is null) return;
        if (!MessageBoxHelper.Confirm(Strings.ConfirmDelete))
            return;

        await _tableService.DeleteTableAsync(SelectedTable.Id);
        SelectedTable = null;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteTableItemAsync(CustomTable? table)
    {
        if (table is null) return;
        if (!MessageBoxHelper.Confirm(Strings.ConfirmDelete))
            return;

        await _tableService.DeleteTableAsync(table.Id);
        await LoadAsync();
    }
}
