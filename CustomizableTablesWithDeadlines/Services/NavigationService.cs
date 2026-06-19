using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.Services;

public class NavigationService : INavigationService
{
    private readonly Func<Guid, object> _tableEditorFactory;

    public NavigationService(Func<Guid, object> tableEditorFactory)
    {
        _tableEditorFactory = tableEditorFactory;
    }

    public event Action<object>? Navigated;

    public object? CurrentViewModel { get; private set; }

    public void NavigateTo(object viewModel)
    {
        CurrentViewModel = viewModel;
        Navigated?.Invoke(viewModel);
    }

    public void NavigateToTableEditor(Guid tableId) =>
        NavigateTo(_tableEditorFactory(tableId));
}
