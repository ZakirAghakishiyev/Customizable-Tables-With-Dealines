using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Services.Interfaces;

public interface ILocalizationService
{
    event EventHandler? LanguageChanged;

    AppLanguage CurrentLanguage { get; }
    string GetString(string key);
    void SetLanguage(AppLanguage language);
}
