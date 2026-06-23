using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class MainViewModel : LocalizedViewModelBase
{
    private readonly INavigationService _navigationService;
    private bool _suppressSelectionNavigation;

    [ObservableProperty] private object? _currentView;
    [ObservableProperty] private string _pageTitle = string.Empty;
    [ObservableProperty] private NavigationItem? _selectedNavigationItem;
    [ObservableProperty] private ObservableCollection<NavigationItem> _navigationItems = [];

    public object? CurrentViewModel => CurrentView;

    public MainViewModel(
        ILocalizationService localization,
        INavigationService navigationService) : base(localization)
    {
        _navigationService = navigationService;

        localization.LanguageChanged += (_, _) => BuildNavigation();

        _navigationService.Navigated += vm =>
        {
            CurrentView = vm;
            if (vm is TableEditorViewModel editor)
                PageTitle = $"{Strings.TableEditor} — {editor.TableName}";
            else if (SelectedNavigationItem is not null)
                PageTitle = SelectedNavigationItem.Title;
        };

        BuildNavigation();
        NavigateDashboardCommand.Execute(null);
    }

    private void BuildNavigation()
    {
        var items = new[]
        {
            new NavigationItem { Key = "Dashboard", Title = Strings.Dashboard, Icon = "\uE80F", NavigateCommand = NavigateDashboardCommand },
            new NavigationItem { Key = "Tables", Title = Strings.Tables, Icon = "\uE8A5", NavigateCommand = NavigateTablesCommand },
            new NavigationItem { Key = "Import", Title = Strings.ImportFromWord, Icon = "\uE8B5", NavigateCommand = NavigateImportCommand },
            new NavigationItem { Key = "Deadlines", Title = Strings.Deadlines, Icon = "\uE823", NavigateCommand = NavigateDeadlinesCommand },
            new NavigationItem { Key = "NotificationSettings", Title = Strings.NotificationSettings, Icon = "\uE7ED", NavigateCommand = NavigateNotificationSettingsCommand },
            new NavigationItem { Key = "Settings", Title = Strings.Settings, Icon = "\uE713", NavigateCommand = NavigateSettingsCommand }
        };

        var currentKey = SelectedNavigationItem?.Key;
        _suppressSelectionNavigation = true;
        try
        {
            NavigationItems = new ObservableCollection<NavigationItem>(items);
            SelectedNavigationItem = NavigationItems.FirstOrDefault(i => i.Key == currentKey) ?? NavigationItems[0];
        }
        finally
        {
            _suppressSelectionNavigation = false;
        }
    }

    partial void OnSelectedNavigationItemChanged(NavigationItem? value)
    {
        if (_suppressSelectionNavigation)
            return;

        value?.NavigateCommand?.Execute(null);
    }

    [RelayCommand]
    private async Task NavigateDashboardAsync()
    {
        _navigationService.NavigateTo<DashboardViewModel>();
        PageTitle = Strings.Dashboard;
        if (CurrentView is DashboardViewModel dashboard)
            await dashboard.LoadAsync();
    }

    [RelayCommand]
    private async Task NavigateTablesAsync()
    {
        _navigationService.NavigateTo<TablesViewModel>();
        PageTitle = Strings.Tables;
        if (CurrentView is TablesViewModel tables)
            await tables.LoadAsync();
    }

    [RelayCommand]
    private void NavigateImport()
    {
        _navigationService.NavigateTo<ImportViewModel>();
        PageTitle = Strings.ImportFromWord;
    }

    [RelayCommand]
    private async Task NavigateDeadlinesAsync()
    {
        _navigationService.NavigateTo<DeadlinesViewModel>();
        PageTitle = Strings.Deadlines;
        if (CurrentView is DeadlinesViewModel deadlines)
            await deadlines.LoadAsync();
    }

    [RelayCommand]
    private async Task NavigateNotificationSettingsAsync()
    {
        _navigationService.NavigateTo<NotificationSettingsViewModel>();
        PageTitle = Strings.NotificationSettings;
        if (CurrentView is NotificationSettingsViewModel notification)
            await notification.LoadAsync();
    }

    [RelayCommand]
    private async Task NavigateSettingsAsync()
    {
        _navigationService.NavigateTo<SettingsViewModel>();
        PageTitle = Strings.Settings;
        if (CurrentView is SettingsViewModel settings)
            await settings.LoadAsync();
    }
}
