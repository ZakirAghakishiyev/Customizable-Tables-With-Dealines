using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class DashboardViewModel : LocalizedViewModelBase
{
    private readonly ITableService _tableService;
    private readonly IDeadlineService _deadlineService;

    [ObservableProperty] private int _totalTables;
    [ObservableProperty] private int _totalRows;
    [ObservableProperty] private int _upcomingDeadlinesCount;
    [ObservableProperty] private int _overdueDeadlinesCount;
    [ObservableProperty] private ObservableCollection<DeadlineListItem> _nearestDeadlines = [];

    public DashboardViewModel(
        ILocalizationService localization,
        ITableService tableService,
        IDeadlineService deadlineService) : base(localization)
    {
        _tableService = tableService;
        _deadlineService = deadlineService;
        _tableService.TablesChanged += async (_, _) => await LoadAsync();
    }

    public async Task LoadAsync()
    {
        var tables = await _tableService.GetAllTablesAsync();
        TotalTables = tables.Count;
        TotalRows = tables.Sum(t => t.RowCount);
        UpcomingDeadlinesCount = await _deadlineService.GetUpcomingCountAsync();
        OverdueDeadlinesCount = await _deadlineService.GetOverdueCountAsync();

        var upcoming = await _deadlineService.GetUpcomingDeadlinesAsync(10);
        NearestDeadlines = new ObservableCollection<DeadlineListItem>(upcoming);
    }
}
