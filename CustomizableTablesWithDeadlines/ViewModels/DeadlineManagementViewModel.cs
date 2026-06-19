using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class DeadlineManagementViewModel : LocalizedViewModelBase
{
    private readonly TableRowData _row;

    [ObservableProperty] private ObservableCollection<DeadlineItem> _deadlines = [];
    [ObservableProperty] private DeadlineItem? _selectedDeadline;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private DateTime _deadlineDateTime = DateTime.Now.AddDays(1);
    [ObservableProperty] private NotifyBeforeOption _notifyBefore = NotifyBeforeOption.OneHour;
    [ObservableProperty] private int _customNotifyMinutes = 30;
    [ObservableProperty] private bool _isEditing;

    [ObservableProperty] private bool _isCustomNotify;

    public Array NotifyOptions => Enum.GetValues<NotifyBeforeOption>();

    public DeadlineManagementViewModel(ILocalizationService localization, TableRowData row) : base(localization)
    {
        _row = row;
        Deadlines = new ObservableCollection<DeadlineItem>(row.Deadlines);
        UpdateCustomNotifyVisibility();
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
    private void DeleteDeadline()
    {
        if (SelectedDeadline is null) return;
        if (!MessageBoxHelper.Confirm(Strings.ConfirmDelete))
            return;

        _row.Deadlines.Remove(SelectedDeadline);
        Deadlines.Remove(SelectedDeadline);
        SelectedDeadline = null;
        ClearForm();
    }

    [RelayCommand]
    private void SaveDeadline()
    {
        if (string.IsNullOrWhiteSpace(Title))
            return;

        if (IsEditing && SelectedDeadline is not null)
        {
            SelectedDeadline.Title = Title.Trim();
            SelectedDeadline.DeadlineDateTime = DeadlineDateTime;
            SelectedDeadline.NotifyBefore = NotifyBefore;
            SelectedDeadline.CustomNotifyMinutes = NotifyBefore == NotifyBeforeOption.Custom
                ? CustomNotifyMinutes
                : null;
            Deadlines[Deadlines.IndexOf(SelectedDeadline)] = SelectedDeadline;
        }
        else
        {
            var item = new DeadlineItem
            {
                Title = Title.Trim(),
                DeadlineDateTime = DeadlineDateTime,
                NotifyBefore = NotifyBefore,
                CustomNotifyMinutes = NotifyBefore == NotifyBeforeOption.Custom
                    ? CustomNotifyMinutes
                    : null
            };
            _row.Deadlines.Add(item);
            Deadlines.Add(item);
        }

        ClearForm();
        IsEditing = false;
    }

    [RelayCommand]
    private void SaveAndClose(System.Windows.Window? window)
    {
        SaveDeadline();
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
