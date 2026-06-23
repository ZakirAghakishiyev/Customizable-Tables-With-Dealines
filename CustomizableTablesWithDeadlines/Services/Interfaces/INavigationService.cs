namespace CustomizableTablesWithDeadlines.Services.Interfaces;

public interface INavigationService
{
    event Action<object>? Navigated;

    object? CurrentView { get; }
    object? CurrentViewModel { get; }

    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateTo(object viewModel);
    void NavigateToTableEditor(int tableId);
}
