using System.Globalization;
using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Services;

public static class ColumnCellValueConverter
{
    public static bool TryConvert(object? value, ColumnType targetType, out object? converted)
    {
        if (IsEmpty(value))
        {
            converted = GetEmptyValue(targetType);
            return true;
        }

        return targetType switch
        {
            ColumnType.Text => TryConvertToText(value!, out converted),
            ColumnType.Number => TryConvertToNumber(value!, out converted),
            ColumnType.DateTime => TryConvertToDateTime(value!, out converted),
            ColumnType.Boolean => TryConvertToBoolean(value!, out converted),
            _ => Assign(false, null, out converted)
        };
    }

    public static object ToDataRowValue(object? value, ColumnType type)
    {
        if (!TryConvert(value, type, out var converted))
            return DBNull.Value;

        return converted ?? DBNull.Value;
    }

    private static bool IsEmpty(object? value) =>
        value is null
        || value is DBNull
        || value is string s && string.IsNullOrWhiteSpace(s);

    private static object? GetEmptyValue(ColumnType type) => type switch
    {
        ColumnType.Number => 0d,
        ColumnType.Boolean => false,
        ColumnType.DateTime => null,
        _ => string.Empty
    };

    private static bool TryConvertToText(object value, out object? converted)
    {
        converted = value switch
        {
            string s => s,
            DateTime dt => dt.ToString("g", CultureInfo.CurrentCulture),
            bool b => b.ToString(CultureInfo.CurrentCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.CurrentCulture),
            _ => value.ToString()
        };

        return true;
    }

    private static bool TryConvertToNumber(object value, out object? converted)
    {
        converted = null;

        switch (value)
        {
            case double d:
                converted = d;
                return true;
            case float f:
                converted = (double)f;
                return true;
            case decimal m:
                converted = (double)m;
                return true;
            case int i:
                converted = (double)i;
                return true;
            case long l:
                converted = (double)l;
                return true;
            case bool b:
                converted = b ? 1d : 0d;
                return true;
            case DateTime:
                return Assign(false, null, out converted);
            case string s when double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out var currentCultureNumber):
                converted = currentCultureNumber;
                return true;
            case string s when double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var invariantCultureNumber):
                converted = invariantCultureNumber;
                return true;
            default:
                return Assign(false, null, out converted);
        }
    }

    private static bool TryConvertToDateTime(object value, out object? converted)
    {
        converted = null;

        if (value is DateTime dt)
        {
            converted = dt;
            return true;
        }

        if (value is string s && TryParseDateTime(s, out var parsed))
        {
            converted = parsed;
            return true;
        }

        return Assign(false, null, out converted);
    }

    private static bool TryConvertToBoolean(object value, out object? converted)
    {
        converted = null;

        switch (value)
        {
            case bool b:
                converted = b;
                return true;
            case double d:
                converted = d != 0d;
                return true;
            case float f:
                converted = f != 0f;
                return true;
            case decimal m:
                converted = m != 0m;
                return true;
            case int i:
                converted = i != 0;
                return true;
            case string s when TryParseBoolean(s, out var boolean):
                converted = boolean;
                return true;
            default:
                return Assign(false, null, out converted);
        }
    }

    private static bool TryParseDateTime(string value, out DateTime result)
    {
        if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out result))
            return true;

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result);
    }

    private static bool TryParseBoolean(string value, out bool result)
    {
        result = false;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Trim().ToLowerInvariant() switch
        {
            "true" or "yes" or "y" or "1" => SetBool(true, out result),
            "false" or "no" or "n" or "0" => SetBool(false, out result),
            _ => bool.TryParse(value, out result)
        };
    }

    private static bool SetBool(bool value, out bool result)
    {
        result = value;
        return true;
    }

    private static bool Assign(bool success, object? value, out object? converted)
    {
        converted = value;
        return success;
    }
}
