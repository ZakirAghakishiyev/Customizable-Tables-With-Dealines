using System.Windows;
using CustomizableTablesWithDeadlines.Localization;
using CustomizableTablesWithDeadlines.Services;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using CustomizableTablesWithDeadlines.Services.Mock;
using CustomizableTablesWithDeadlines.ViewModels;

namespace CustomizableTablesWithDeadlines;

public partial class App : Application
{
    private static readonly Dictionary<Type, object> Services = new();

    public static ILocalizationService Localization => GetService<ILocalizationService>();
    public static LocalizedStrings Strings => GetService<LocalizedStrings>();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ConfigureServices();

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    private static void ConfigureServices()
    {
        var localization = new LocalizationService();
        Register(localization);
        Register<ILocalizationService>(localization);
        Register(new LocalizedStrings(localization));

        var tableService = new MockTableService();
        Register<ITableService>(tableService);

        var wordImportService = new WordImportService();
        Register<IWordImportService>(wordImportService);

        var deadlineService = new MockDeadlineService(tableService);
        Register<IDeadlineService>(deadlineService);

        var notificationSettingsService = new MockNotificationSettingsService();
        Register<INotificationSettingsService>(notificationSettingsService);

        TableEditorViewModel CreateTableEditor(Guid tableId)
        {
            var vm = new TableEditorViewModel(
                localization,
                tableService,
                GetService<INavigationService>(),
                row => new DeadlineManagementViewModel(localization, row));
            vm.InitializeAsync(tableId).GetAwaiter().GetResult();
            return vm;
        }

        var navigationService = new NavigationService(CreateTableEditor);
        Register<INavigationService>(navigationService);

        Register(new TablesViewModel(localization, tableService, wordImportService, navigationService));
        Register(new DashboardViewModel(localization, tableService, deadlineService));
        Register(new DeadlinesViewModel(localization, deadlineService));
        Register(new NotificationSettingsViewModel(localization, notificationSettingsService));
        Register(new SettingsViewModel(localization));

        Register(new MainViewModel(
            localization,
            navigationService,
            GetService<DashboardViewModel>(),
            GetService<TablesViewModel>(),
            GetService<DeadlinesViewModel>(),
            GetService<NotificationSettingsViewModel>(),
            GetService<SettingsViewModel>()));
    }

    public static T GetService<T>() where T : class =>
        Services[typeof(T)] as T ?? throw new InvalidOperationException($"Service {typeof(T).Name} not registered.");

    private static void Register<T>(T instance) where T : class => Services[typeof(T)] = instance;
}
