using System.Windows.Input;

namespace CustomizableTablesWithDeadlines.ViewModels;

public class NavigationItem
{
    public required string Key { get; init; }
    public required string Title { get; init; }
    public required string Icon { get; init; }
    public ICommand? NavigateCommand { get; init; }
}
