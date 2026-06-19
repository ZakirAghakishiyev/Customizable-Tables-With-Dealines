using System.ComponentModel;
using System.Globalization;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.Localization;

public class LocalizedStrings : INotifyPropertyChanged
{
    private readonly ILocalizationService _localization;

    public LocalizedStrings(ILocalizationService localization)
    {
        _localization = localization;
        _localization.LanguageChanged += (_, _) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] => _localization.GetString(key);

    public string Dashboard => this["Dashboard"];
    public string Tables => this["Tables"];
    public string Deadlines => this["Deadlines"];
    public string NotificationSettings => this["NotificationSettings"];
    public string Settings => this["Settings"];
    public string CreateNewTable => this["CreateNewTable"];
    public string AddColumn => this["AddColumn"];
    public string AddRow => this["AddRow"];
    public string Save => this["Save"];
    public string Cancel => this["Cancel"];
    public string Delete => this["Delete"];
    public string Edit => this["Edit"];
    public string Language => this["Language"];
    public string AppTitle => this["AppTitle"];
    public string TotalTables => this["TotalTables"];
    public string TotalRows => this["TotalRows"];
    public string UpcomingDeadlines => this["UpcomingDeadlines"];
    public string OverdueDeadlines => this["OverdueDeadlines"];
    public string NearestUpcomingDeadlines => this["NearestUpcomingDeadlines"];
    public string Open => this["Open"];
    public string RenameTable => this["RenameTable"];
    public string DeleteTable => this["DeleteTable"];
    public string Columns => this["Columns"];
    public string Rows => this["Rows"];
    public string LastUpdated => this["LastUpdated"];
    public string RenameColumn => this["RenameColumn"];
    public string DeleteColumn => this["DeleteColumn"];
    public string MoveColumnLeft => this["MoveColumnLeft"];
    public string MoveColumnRight => this["MoveColumnRight"];
    public string ColumnType => this["ColumnType"];
    public string Text => this["Text"];
    public string Number => this["Number"];
    public string DateTime => this["DateTime"];
    public string Boolean => this["Boolean"];
    public string ManageDeadlines => this["ManageDeadlines"];
    public string DeleteRow => this["DeleteRow"];
    public string TableName => this["TableName"];
    public string RowInfo => this["RowInfo"];
    public string DeadlineTitle => this["DeadlineTitle"];
    public string DeadlineDateTime => this["DeadlineDateTime"];
    public string RemainingTime => this["RemainingTime"];
    public string Status => this["Status"];
    public string FilterAll => this["FilterAll"];
    public string FilterToday => this["FilterToday"];
    public string FilterThisWeek => this["FilterThisWeek"];
    public string FilterOverdue => this["FilterOverdue"];
    public string StatusUpcoming => this["StatusUpcoming"];
    public string StatusDueSoon => this["StatusDueSoon"];
    public string StatusOverdue => this["StatusOverdue"];
    public string StatusCompleted => this["StatusCompleted"];
    public string DefaultNotifyBefore => this["DefaultNotifyBefore"];
    public string EnableDesktopNotifications => this["EnableDesktopNotifications"];
    public string EnableSound => this["EnableSound"];
    public string StartWithWindows => this["StartWithWindows"];
    public string Theme => this["Theme"];
    public string ThemePlaceholder => this["ThemePlaceholder"];
    public string BackupExport => this["BackupExport"];
    public string BackupExportPlaceholder => this["BackupExportPlaceholder"];
    public string About => this["About"];
    public string AboutDescription => this["AboutDescription"];
    public string English => this["English"];
    public string Azerbaijani => this["Azerbaijani"];
    public string Russian => this["Russian"];
    public string AddDeadline => this["AddDeadline"];
    public string EditDeadline => this["EditDeadline"];
    public string DeleteDeadline => this["DeleteDeadline"];
    public string NotificationBefore => this["NotificationBefore"];
    public string TenMinutes => this["TenMinutes"];
    public string ThirtyMinutes => this["ThirtyMinutes"];
    public string OneHour => this["OneHour"];
    public string ThreeHours => this["ThreeHours"];
    public string OneDay => this["OneDay"];
    public string CustomMinutes => this["CustomMinutes"];
    public string EnterName => this["EnterName"];
    public string ConfirmDelete => this["ConfirmDelete"];
    public string TableEditor => this["TableEditor"];
    public string BackToTables => this["BackToTables"];
    public string NoTablesYet => this["NoTablesYet"];
    public string NoDeadlinesYet => this["NoDeadlinesYet"];
    public string SelectTable => this["SelectTable"];
    public string SelectColumn => this["SelectColumn"];
    public string SelectDeadline => this["SelectDeadline"];
    public string Version => this["Version"];
    public string WelcomeSubtitle => this["WelcomeSubtitle"];
    public string Actions => this["Actions"];
    public string General => this["General"];
    public string Appearance => this["Appearance"];
    public string Data => this["Data"];
    public string ImportFromWord => this["ImportFromWord"];
    public string Import => this["Import"];
    public string WordDocumentFilter => this["WordDocumentFilter"];
    public string NoTablesFoundInWord => this["NoTablesFoundInWord"];
    public string ImportWordFailed => this["ImportWordFailed"];
    public string ImportWordPreviewDescription => this["ImportWordPreviewDescription"];
    public string SelectTableToImport => this["SelectTableToImport"];
    public string TablePreview => this["TablePreview"];
    public string SuggestedDeadlineColumns => this["SuggestedDeadlineColumns"];
    public string SuggestedDeadlineColumnsHint => this["SuggestedDeadlineColumnsHint"];
    public string NoDeadlineColumnsDetected => this["NoDeadlineColumnsDetected"];

    public string GetStatusText(Models.Enums.DeadlineStatus status) => status switch
    {
        Models.Enums.DeadlineStatus.Upcoming => StatusUpcoming,
        Models.Enums.DeadlineStatus.DueSoon => StatusDueSoon,
        Models.Enums.DeadlineStatus.Overdue => StatusOverdue,
        Models.Enums.DeadlineStatus.Completed => StatusCompleted,
        _ => status.ToString()
    };

    public string GetColumnTypeText(Models.Enums.ColumnType type) => type switch
    {
        Models.Enums.ColumnType.Text => Text,
        Models.Enums.ColumnType.Number => Number,
        Models.Enums.ColumnType.DateTime => DateTime,
        Models.Enums.ColumnType.Boolean => Boolean,
        _ => type.ToString()
    };

    public string GetNotifyBeforeText(Models.Enums.NotifyBeforeOption option) => option switch
    {
        Models.Enums.NotifyBeforeOption.TenMinutes => TenMinutes,
        Models.Enums.NotifyBeforeOption.ThirtyMinutes => ThirtyMinutes,
        Models.Enums.NotifyBeforeOption.OneHour => OneHour,
        Models.Enums.NotifyBeforeOption.ThreeHours => ThreeHours,
        Models.Enums.NotifyBeforeOption.OneDay => OneDay,
        Models.Enums.NotifyBeforeOption.Custom => CustomMinutes,
        _ => option.ToString()
    };

    public string FormatDate(DateTime date) =>
        date.ToString("g", CultureInfo.CurrentUICulture);
}
