using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public partial class SettingsViewModel : LocalizedViewModelBase
{
    private readonly ILocalizationService _localizationService;

    [ObservableProperty] private AppLanguage _selectedLanguage;

    public SettingsViewModel(ILocalizationService localizationService) : base(localizationService)
    {
        _localizationService = localizationService;
        _selectedLanguage = localizationService.CurrentLanguage;
    }

    public AppLanguage[] Languages { get; } =
    [
        AppLanguage.English,
        AppLanguage.Azerbaijani,
        AppLanguage.Russian
    ];

    partial void OnSelectedLanguageChanged(AppLanguage value)
    {
        _localizationService.SetLanguage(value);
    }

    public string GetLanguageDisplayName(AppLanguage language) => language switch
    {
        AppLanguage.Azerbaijani => Strings.Azerbaijani,
        AppLanguage.Russian => Strings.Russian,
        _ => Strings.English
    };
}
