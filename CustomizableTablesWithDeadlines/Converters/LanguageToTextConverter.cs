using System.Globalization;
using System.Windows.Data;
using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Converters;

public class LanguageToTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AppLanguage language)
            return value?.ToString() ?? string.Empty;

        var strings = LocalizationHelper.Strings;
        return language switch
        {
            AppLanguage.Azerbaijani => strings.Azerbaijani,
            AppLanguage.Russian => strings.Russian,
            _ => strings.English
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
