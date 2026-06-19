using System.Globalization;
using System.Resources;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.Localization;

public class LocalizationService : ILocalizationService
{
    private static readonly ResourceManager ResourceManager = new(
        $"{typeof(LocalizationService).Assembly.GetName().Name}.Resources.Strings",
        typeof(LocalizationService).Assembly);

    private CultureInfo _culture = CultureInfo.GetCultureInfo("en");

    public event EventHandler? LanguageChanged;

    public AppLanguage CurrentLanguage { get; private set; } = AppLanguage.English;

    public string GetString(string key) =>
        ResourceManager.GetString(key, _culture) ?? key;

    public void SetLanguage(AppLanguage language)
    {
        CurrentLanguage = language;
        _culture = language switch
        {
            AppLanguage.Azerbaijani => CultureInfo.GetCultureInfo("az"),
            AppLanguage.Russian => CultureInfo.GetCultureInfo("ru"),
            _ => CultureInfo.GetCultureInfo("en")
        };

        Thread.CurrentThread.CurrentUICulture = _culture;
        CultureInfo.CurrentUICulture = _culture;
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
