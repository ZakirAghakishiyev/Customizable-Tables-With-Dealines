using CommunityToolkit.Mvvm.ComponentModel;

namespace CustomizableTablesWithDeadlines.Models;

public partial class DeadlineColumnSuggestion : ObservableObject
{
    public int ColumnIndex { get; init; }
    public string ColumnName { get; init; } = string.Empty;

    [ObservableProperty] private bool _isSelected = true;
}
