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
    }

    public async Task LoadAsync()
    {
        var tables = await _tableService.GetAllTablesAsync();
        Tables = new ObservableCollection<CustomTable>(tables);
    }

    [RelayCommand]
    private void ImportFromWord() => _navigationService.NavigateTo<ImportViewModel>();

    [RelayCommand]
    private void SelectTable(CustomTable? table)
    {
        SelectedTable = table;
    }

    [RelayCommand]
    private async Task CreateTableAsync()
    {
        if (!InputDialog.TryPrompt(Strings.CreateNewTable, Strings.TableName, out var name))
            return;

        try
        {
            await _tableService.CreateTableAsync(name);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowError(ex.Message);
        }
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
        await RenameTableItemAsync(SelectedTable);
    }

    [RelayCommand]
    private async Task RenameTableItemAsync(CustomTable? table)
    {
        if (table is null) return;

        if (!InputDialog.TryPrompt(Strings.RenameTable, Strings.TableName, out var newName, table.Name))
            return;

        if (string.Equals(table.Name, newName, StringComparison.Ordinal))
            return;

        try
        {
            await _tableService.RenameTableAsync(table.Id, newName);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowError(ex.Message);
        }
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
