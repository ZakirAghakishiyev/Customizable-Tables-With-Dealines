using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Localization;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class DeadlinesViewModel : LocalizedViewModelBase
{
    private readonly IDeadlineService _deadlineService;
    private List<DeadlineListItem> _allDeadlines = [];

    [ObservableProperty] private ObservableCollection<DeadlineDisplayItem> _deadlines = [];
    [ObservableProperty] private DeadlineFilter _selectedFilter = DeadlineFilter.All;

    public Array FilterOptions => Enum.GetValues<DeadlineFilter>();

    public DeadlinesViewModel(
        ILocalizationService localization,
        IDeadlineService deadlineService) : base(localization)
    {
        _deadlineService = deadlineService;
    }

    public async Task LoadAsync()
    {
        _allDeadlines = (await _deadlineService.GetAllDeadlinesAsync()).ToList();
        ApplyFilter();
    }

    partial void OnSelectedFilterChanged(DeadlineFilter value) => ApplyFilter();

    private void ApplyFilter()
    {
        var filtered = Services.Mock.MockDeadlineService.FilterDeadlines(_allDeadlines, SelectedFilter);
        Deadlines = new ObservableCollection<DeadlineDisplayItem>(
            filtered.Select(d => new DeadlineDisplayItem(d, Strings)));
    }

    public string GetFilterText(DeadlineFilter filter) => filter switch
    {
        DeadlineFilter.Today => Strings.FilterToday,
        DeadlineFilter.ThisWeek => Strings.FilterThisWeek,
        DeadlineFilter.Overdue => Strings.FilterOverdue,
        _ => Strings.FilterAll
    };
}

public class DeadlineDisplayItem
{
    private readonly LocalizedStrings _strings;

    public DeadlineDisplayItem(DeadlineListItem source, LocalizedStrings strings)
    {
        _strings = strings;
        Source = source;
    }

    public DeadlineListItem Source { get; }
    public string TableName => Source.TableName;
    public string RowInfo => Source.RowInfo;
    public string Title => Source.Title;
    public string DeadlineDateTimeText => _strings.FormatDate(Source.DeadlineDateTime);
    public string RemainingTimeText => DeadlineHelper.FormatRemainingTime(Source.Remaining, Source.IsCompleted);
    public string StatusText => _strings.GetStatusText(Source.Status);
    public DeadlineStatus Status => Source.Status;
}
