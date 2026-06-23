using CustomizableTablesWithDeadlines.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CustomizableTablesWithDeadlines.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Func<int, object> _tableEditorFactory;

    public NavigationService(IServiceProvider serviceProvider, Func<int, object> tableEditorFactory)
    {
        _serviceProvider = serviceProvider;
        _tableEditorFactory = tableEditorFactory;
    }

    public event Action<object>? Navigated;

    public object? CurrentView { get; private set; }

    public object? CurrentViewModel => CurrentView;

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        NavigateTo(viewModel);
    }

    public void NavigateTo(object viewModel)
    {
        CurrentView = viewModel;
        Navigated?.Invoke(viewModel);
    }

    public void NavigateToTableEditor(int tableId) =>
        NavigateTo(_tableEditorFactory(tableId));
}
