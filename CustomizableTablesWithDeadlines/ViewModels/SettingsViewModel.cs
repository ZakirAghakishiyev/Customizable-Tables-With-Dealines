using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class SettingsViewModel : LocalizedViewModelBase
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty] private AppLanguage _selectedLanguage;

    public SettingsViewModel(
        ILocalizationService localizationService,
        ISettingsService settingsService) : base(localizationService)
    {
        _localizationService = localizationService;
        _settingsService = settingsService;
        _selectedLanguage = localizationService.CurrentLanguage;
    }

    public AppLanguage[] Languages { get; } =
    [
        AppLanguage.English,
        AppLanguage.Azerbaijani,
        AppLanguage.Russian
    ];

    public async Task LoadAsync()
    {
        var settings = await _settingsService.GetAsync();
        await _localizationService.SetLanguageAsync(settings.Language);
        SelectedLanguage = _localizationService.CurrentLanguage;
    }

    partial void OnSelectedLanguageChanged(AppLanguage value)
    {
        _ = ApplyLanguageAsync(value);
    }

    private async Task ApplyLanguageAsync(AppLanguage language)
    {
        var code = language switch
        {
            AppLanguage.Azerbaijani => "az",
            AppLanguage.Russian => "ru",
            _ => "en"
        };

        await _localizationService.SetLanguageAsync(code);

        var settings = await _settingsService.GetAsync();
        settings.Language = code;
        await _settingsService.UpdateAsync(settings);
    }

    public string GetLanguageDisplayName(AppLanguage language) => language switch
    {
        AppLanguage.Azerbaijani => Strings.Azerbaijani,
        AppLanguage.Russian => Strings.Russian,
        _ => Strings.English
    };
}