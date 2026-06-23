using System.Collections.ObjectModel;
using System.Windows;
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
    [ObservableProperty] private DateTime? _deadlineDateTime = DateTime.Today.AddDays(1);
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

    public async Task InitializeAsync(int rowId)
    {
        _rowId = rowId;
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        var items = await _deadlineService.GetByRowIdAsync(_rowId);
        Deadlines = new ObservableCollection<DeadlineItem>(
            items.Select(dto =>
            {
                var item = PresentationMapping.ToDeadlineItem(dto);
                item.NotifyBeforeText = Strings.GetNotifyBeforeText(item.NotifyBefore);
                return item;
            }));
    }

    partial void OnNotifyBeforeChanged(NotifyBeforeOption value) => UpdateCustomNotifyVisibility();

    private void UpdateCustomNotifyVisibility() =>
        IsCustomNotify = NotifyBefore == NotifyBeforeOption.Custom;

    [RelayCommand]
    private void AddDeadline()
    {
        SelectedDeadline = null;
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

        try
        {
            await _deadlineService.DeleteAsync(SelectedDeadline.Id);
            await LoadAsync();
            SelectedDeadline = null;
            ClearForm();
            IsEditing = false;
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowError(ex.Message);
        }
    }

    [RelayCommand]
    private async Task SaveAndCloseAsync(Window? window)
    {
        if (!await TrySaveAsync())
            return;

        if (window is not null)
        {
            window.DialogResult = true;
            window.Close();
        }
    }

    [RelayCommand]
    private void Cancel(Window? window)
    {
        if (window is not null)
        {
            window.DialogResult = false;
            window.Close();
        }
    }

    private async Task<bool> TrySaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            MessageBoxHelper.ShowError(Strings.EnterName);
            return false;
        }

        if (DeadlineDateTime is null)
        {
            MessageBoxHelper.ShowError(Strings.DeadlineDateTime);
            return false;
        }

        try
        {
            if (IsEditing && SelectedDeadline is not null)
            {
                await _deadlineService.UpdateAsync(new UpdateDeadlineDto
                {
                    Id = SelectedDeadline.Id,
                    Title = Title.Trim(),
                    DeadlineDateTime = DeadlineDateTime.Value
                });
            }
            else
            {
                var id = await _deadlineService.CreateAsync(new CreateDeadlineDto
                {
                    RowId = _rowId,
                    Title = Title.Trim(),
                    DeadlineDateTime = DeadlineDateTime.Value
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
            return true;
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowError(ex.Message);
            return false;
        }
    }

    private void ClearForm()
    {
        Title = string.Empty;
        DeadlineDateTime = DateTime.Today.AddDays(1);
        NotifyBefore = NotifyBeforeOption.OneHour;
        CustomNotifyMinutes = 30;
    }

    public string GetNotifyOptionText(NotifyBeforeOption option) => Strings.GetNotifyBeforeText(option);
}
