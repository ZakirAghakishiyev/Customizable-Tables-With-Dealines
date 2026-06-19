using CommunityToolkit.Mvvm.ComponentModel;
using CustomizableTablesWithDeadlines.Localization;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.ViewModels;

public abstract class LocalizedViewModelBase : ObservableObject
{
    protected LocalizedViewModelBase(ILocalizationService localization)
    {
        Strings = new LocalizedStrings(localization);
        localization.LanguageChanged += (_, _) => OnPropertyChanged(nameof(Strings));
    }

    public LocalizedStrings Strings { get; }
}
