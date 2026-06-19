namespace CustomizableTablesWithDeadlines.Services.Interfaces;

public interface INavigationService
{
    event Action<object>? Navigated;

    object? CurrentViewModel { get; }
    void NavigateTo(object viewModel);
    void NavigateToTableEditor(Guid tableId);
}
