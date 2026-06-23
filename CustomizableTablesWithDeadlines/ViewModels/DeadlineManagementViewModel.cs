using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.DTOs.Deadlines;
using CustomizableTablesWithDeadlines.Application.DTOs.Notifications;
using CustomizableTablesWithDeadlines.Mapping;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using AppDeadlineService = CustomizableTablesWithDeadlines.Application.Abstractions.Services.IDeadlineService;
using AppNotificationRuleService = CustomizableTablesWithDeadlines.Application.Abstractions.Services.INotificationRuleService;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class DeadlineManagementViewModel : LocalizedViewModelBase
{
    private readonly AppDeadlineService _deadlineService;
    private readonly AppNotificationRuleService _notificationRuleService;
    private int _rowId;

    [ObservableProperty] private ObservableCollection<DeadlineItem> _deadlines = [];
    [ObservableProperty] private DeadlineItem? _selectedDeadline;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private DateTime _deadlineDateTime = DateTime.Now.AddDays(1);
    [ObservableProperty] private NotifyBeforeOption _notifyBefore = NotifyBeforeOption.OneHour;
    [ObservableProperty] private int _customNotifyMinutes = 30;
    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private bool _isCustomNotify;

    public Array NotifyOptions => Enum.GetValues<NotifyBeforeOption>();

    public DeadlineManagementViewModel(
        ILocalizationService localization,
        AppDeadlineService deadlineService,
        AppNotificationRuleService notificationRuleService) : base(localization)
    {
        _deadlineService = deadlineService;
        _notificationRuleService = notificationRuleService;
        UpdateCustomNotifyVisibility();
    }

    public async void Initialize(int rowId)
    {
        _rowId = rowId;
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        var items = await _deadlineService.GetByRowIdAsync(_rowId);
        Deadlines = new ObservableCollection<DeadlineItem>(items.Select(PresentationMapping.ToDeadlineItem));
    }

    partial void OnNotifyBeforeChanged(NotifyBeforeOption value) => UpdateCustomNotifyVisibility();

    private void UpdateCustomNotifyVisibility() =>
        IsCustomNotify = NotifyBefore == NotifyBeforeOption.Custom;

    [RelayCommand]
    private void AddDeadline()
    {
        ClearForm();
        IsEditing = false;
    }

    [RelayCommand]
    private void EditDeadline()
    {
        if (SelectedDeadline is null) return;

        Title = SelectedDeadline.Title;
        DeadlineDateTime = SelectedDeadline.DeadlineDateTime;
        NotifyBefore = SelectedDeadline.NotifyBefore;
        CustomNotifyMinutes = SelectedDeadline.CustomNotifyMinutes ?? 30;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task DeleteDeadlineAsync()
    {
        if (SelectedDeadline is null) return;
        if (!MessageBoxHelper.Confirm(Strings.ConfirmDelete))
            return;

        await _deadlineService.DeleteAsync(SelectedDeadline.Id);
        await LoadAsync();
        SelectedDeadline = null;
        ClearForm();
    }

    [RelayCommand]
    private async Task SaveDeadlineAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
            return;

        if (IsEditing && SelectedDeadline is not null)
        {
            await _deadlineService.UpdateAsync(new UpdateDeadlineDto
            {
                Id = SelectedDeadline.Id,
                Title = Title.Trim(),
                DeadlineDateTime = DeadlineDateTime
            });
        }
        else
        {
            var id = await _deadlineService.CreateAsync(new CreateDeadlineDto
            {
                RowId = _rowId,
                Title = Title.Trim(),
                DeadlineDateTime = DeadlineDateTime
            });

            await _notificationRuleService.CreateAsync(new CreateNotificationRuleDto
            {
                DeadlineId = id,
                NotifyBeforeMinutes = PresentationMapping.NotifyOptionToMinutes(NotifyBefore, CustomNotifyMinutes),
                IsEnabled = true
            });
        }

        await LoadAsync();
        ClearForm();
        IsEditing = false;
    }

    [RelayCommand]
    private async Task SaveAndCloseAsync(System.Windows.Window? window)
    {
        await SaveDeadlineAsync();
        window?.Close();
    }

    private void ClearForm()
    {
        Title = string.Empty;
        DeadlineDateTime = DateTime.Now.AddDays(1);
        NotifyBefore = NotifyBeforeOption.OneHour;
        CustomNotifyMinutes = 30;
    }

    public string GetNotifyOptionText(NotifyBeforeOption option) => Strings.GetNotifyBeforeText(option);
}
