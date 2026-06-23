using System.Windows;
using CustomizableTablesWithDeadlines.Application;
using CustomizableTablesWithDeadlines.Application.Abstractions.Infrastructure;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Infrastructure;
using CustomizableTablesWithDeadlines.Infrastructure.Notifications;
using CustomizableTablesWithDeadlines.Localization;
using CustomizableTablesWithDeadlines.Services.Interfaces;
using CustomizableTablesWithDeadlines.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WpfApplication = System.Windows.Application;

namespace CustomizableTablesWithDeadlines;

public partial class App : WpfApplication
{
    private static ServiceProvider? _rootProvider;
    private static IServiceScope? _appScope;

    public static IServiceProvider Services { get; private set; } = null!;

    public static ILocalizationService Localization => GetService<ILocalizationService>();
    public static LocalizedStrings Strings => GetService<LocalizedStrings>();

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            WindowsToastRegistration.Register();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Toast registration failed: {ex}");
        }

        var configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddLogging(builder => builder.AddDebug());
        serviceCollection.AddApplication();
        serviceCollection.AddInfrastructure(configuration);
        serviceCollection.AddPresentation();

        _rootProvider = serviceCollection.BuildServiceProvider();
        _appScope = _rootProvider.CreateScope();
        Services = _appScope.ServiceProvider;

        try
        {
            var settingsService = Services.GetRequiredService<Application.Abstractions.Services.ISettingsService>();
            var localization = Services.GetRequiredService<ILocalizationService>();
            var settings = await settingsService.GetAsync();
            await localization.SetLanguageAsync(settings.Language);

            var dbInitializer = Services.GetRequiredService<IDatabaseInitializer>();
            await dbInitializer.InitializeAsync();

            try
            {
                var notificationScheduler = Services.GetRequiredService<INotificationScheduler>();
                await notificationScheduler.ScheduleAllFutureNotificationsAsync();
            }
            catch (Exception scheduleEx)
            {
                var logger = Services.GetRequiredService<ILogger<App>>();
                logger.LogError(scheduleEx, "Notification scheduling failed during startup.");
            }

            var mainWindow = Services.GetRequiredService<MainWindow>();
            var mainViewModel = Services.GetRequiredService<MainViewModel>();
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    public static T GetService<T>() where T : class =>
        Services.GetRequiredService<T>();

    protected override void OnExit(ExitEventArgs e)
    {
        _appScope?.Dispose();
        _rootProvider?.Dispose();
        base.OnExit(e);
    }
}
