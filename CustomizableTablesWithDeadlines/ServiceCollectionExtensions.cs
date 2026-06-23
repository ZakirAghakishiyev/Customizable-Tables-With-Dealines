using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Localization;
using CustomizableTablesWithDeadlines.Services;
using CustomizableTablesWithDeadlines.Services.Adapters;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using CustomizableTablesWithDeadlines.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using PresentationDeadlineService = CustomizableTablesWithDeadlines.Services.Interfaces.IDeadlineService;
using PresentationTableService = CustomizableTablesWithDeadlines.Services.Interfaces.ITableService;

namespace CustomizableTablesWithDeadlines;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<IDesktopNotificationFallback, WpfDesktopNotificationFallback>();
        services.AddSingleton<LocalizedStrings>(sp =>
            new LocalizedStrings(sp.GetRequiredService<ILocalizationService>()));

        services.AddScoped<PresentationTableService, TablePresentationService>();
        services.AddScoped<PresentationDeadlineService, DeadlinePresentationService>();
        services.AddScoped<INotificationSettingsService, NotificationSettingsPresentationService>();

        services.AddSingleton<INavigationService>(sp =>
            new NavigationService(sp, tableId =>
            {
                var vm = sp.GetRequiredService<TableEditorViewModel>();
                vm.InitializeAsync(tableId).GetAwaiter().GetResult();
                return vm;
            }));

        services.AddTransient<MainWindow>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<TablesViewModel>();
        services.AddTransient<DeadlinesViewModel>();
        services.AddTransient<NotificationSettingsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ImportViewModel>();
        services.AddTransient<DeadlineManagementViewModel>();

        services.AddTransient<Func<int, Task<DeadlineManagementViewModel>>>(sp => async rowId =>
        {
            var vm = ActivatorUtilities.CreateInstance<DeadlineManagementViewModel>(sp);
            await vm.InitializeAsync(rowId);
            return vm;
        });

        services.AddTransient<TableEditorViewModel>(sp =>
        {
            var localization = sp.GetRequiredService<ILocalizationService>();
            var tableService = sp.GetRequiredService<PresentationTableService>();
            var navigation = sp.GetRequiredService<INavigationService>();
            var deadlineFactory = sp.GetRequiredService<Func<int, Task<DeadlineManagementViewModel>>>();
            return new TableEditorViewModel(
                localization,
                tableService,
                navigation,
                row => deadlineFactory(row.Id));
        });

        return services;
    }
}
