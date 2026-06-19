using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class MainViewModel : LocalizedViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly TablesViewModel _tablesViewModel;
    private readonly DeadlinesViewModel _deadlinesViewModel;
    private readonly NotificationSettingsViewModel _notificationSettingsViewModel;
    private readonly SettingsViewModel _settingsViewModel;

    [ObservableProperty] private object? _currentViewModel;
    [ObservableProperty] private string _pageTitle = string.Empty;
    [ObservableProperty] private NavigationItem? _selectedNavigationItem;
    [ObservableProperty] private ObservableCollection<NavigationItem> _navigationItems = [];

    public MainViewModel(
        ILocalizationService localization,
        INavigationService navigationService,
        DashboardViewModel dashboardViewModel,
        TablesViewModel tablesViewModel,
        DeadlinesViewModel deadlinesViewModel,
        NotificationSettingsViewModel notificationSettingsViewModel,
        SettingsViewModel settingsViewModel) : base(localization)
    {
        _navigationService = navigationService;
        _dashboardViewModel = dashboardViewModel;
        _tablesViewModel = tablesViewModel;
        _deadlinesViewModel = deadlinesViewModel;
        _notificationSettingsViewModel = notificationSettingsViewModel;
        _settingsViewModel = settingsViewModel;

        localization.LanguageChanged += (_, _) => BuildNavigation();

        _navigationService.Navigated += vm =>
        {
            CurrentViewModel = vm;
            if (vm is TableEditorViewModel editor)
                PageTitle = $"{Strings.TableEditor} — {editor.TableName}";
            else if (SelectedNavigationItem is not null)
                PageTitle = SelectedNavigationItem.Title;
        };

        BuildNavigation();
        NavigateTo(_dashboardViewModel);
    }

    private void BuildNavigation()
    {
        var items = new[]
        {
            new NavigationItem { Key = "Dashboard", Title = Strings.Dashboard, Icon = "\uE80F", ViewModel = _dashboardViewModel },
            new NavigationItem { Key = "Tables", Title = Strings.Tables, Icon = "\uE8A5", ViewModel = _tablesViewModel },
            new NavigationItem { Key = "Deadlines", Title = Strings.Deadlines, Icon = "\uE823", ViewModel = _deadlinesViewModel },
            new NavigationItem { Key = "NotificationSettings", Title = Strings.NotificationSettings, Icon = "\uE7ED", ViewModel = _notificationSettingsViewModel },
            new NavigationItem { Key = "Settings", Title = Strings.Settings, Icon = "\uE713", ViewModel = _settingsViewModel }
        };

        var currentKey = SelectedNavigationItem?.Key;
        NavigationItems = new ObservableCollection<NavigationItem>(items);
        SelectedNavigationItem = NavigationItems.FirstOrDefault(i => i.Key == currentKey) ?? NavigationItems[0];
    }

    partial void OnSelectedNavigationItemChanged(NavigationItem? value)
    {
        if (value is not null)
            NavigateTo(value.ViewModel);
    }

    private async void NavigateTo(object viewModel)
    {
        CurrentViewModel = viewModel;

        if (viewModel is DashboardViewModel dashboard)
        {
            PageTitle = Strings.Dashboard;
            await dashboard.LoadAsync();
        }
        else if (viewModel is TablesViewModel tables)
        {
            PageTitle = Strings.Tables;
            await tables.LoadAsync();
        }
        else if (viewModel is DeadlinesViewModel deadlines)
        {
            PageTitle = Strings.Deadlines;
            await deadlines.LoadAsync();
        }
        else if (viewModel is NotificationSettingsViewModel notification)
        {
            PageTitle = Strings.NotificationSettings;
            await notification.LoadAsync();
        }
        else if (viewModel is SettingsViewModel)
        {
            PageTitle = Strings.Settings;
        }
        else if (viewModel is TableEditorViewModel editor)
        {
            PageTitle = $"{Strings.TableEditor} — {editor.TableName}";
        }
    }
}
