namespace CustomizableTablesWithDeadlines.ViewModels;

public class NavigationItem
{
    public required string Key { get; init; }
    public required string Title { get; init; }
    public required string Icon { get; init; }
    public required object ViewModel { get; init; }
}
