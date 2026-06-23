using System.Globalization;
using CustomizableTablesWithDeadlines.Application.DTOs.Rows;
using CustomizableTablesWithDeadlines.Application.Exceptions;
using CustomizableTablesWithDeadlines.Domain.Enums;

namespace CustomizableTablesWithDeadlines.Application.Validators;

public static class CellValueValidator
{
    public static void ApplyValue(Domain.Entities.CellValue cell, ColumnDataType dataType, UpdateCellValueDto dto)
    {
        cell.ValueText = null;
        cell.ValueNumber = null;
        cell.ValueDateTime = null;
        cell.ValueBoolean = null;

        switch (dataType)
        {
            case ColumnDataType.Text:
                cell.ValueText = dto.TextValue ?? string.Empty;
                break;
            case ColumnDataType.Number:
                if (dto.NumberValue is null)
                    throw new InvalidCellValueException("A numeric value is required for this column.");
                cell.ValueNumber = dto.NumberValue;
                break;
            case ColumnDataType.DateTime:
                if (dto.DateTimeValue is null)
                    throw new InvalidCellValueException("A date/time value is required for this column.");
                cell.ValueDateTime = dto.DateTimeValue;
                break;
            case ColumnDataType.Boolean:
                if (dto.BooleanValue is null)
                    throw new InvalidCellValueException("A boolean value is required for this column.");
                cell.ValueBoolean = dto.BooleanValue;
                break;
            default:
                throw new InvalidCellValueException($"Unsupported column data type: {dataType}.");
        }
    }

    public static ColumnDataType DetectDataType(IReadOnlyList<string> values)
    {
        var nonEmpty = values.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
        if (nonEmpty.Count == 0)
            return ColumnDataType.Text;

        var dateCount = nonEmpty.Count(v => TryParseDateTime(v, out _));
        var numberCount = nonEmpty.Count(v =>
            decimal.TryParse(v, NumberStyles.Any, CultureInfo.CurrentCulture, out _)
            || decimal.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out _));
        var boolCount = nonEmpty.Count(v => TryParseBoolean(v, out _));

        const double threshold = 0.6;
        if (dateCount >= nonEmpty.Count * threshold)
            return ColumnDataType.DateTime;
        if (numberCount >= nonEmpty.Count * threshold)
            return ColumnDataType.Number;
        if (boolCount >= nonEmpty.Count * threshold)
            return ColumnDataType.Boolean;

        return ColumnDataType.Text;
    }

    public static void SetFromString(Domain.Entities.CellValue cell, ColumnDataType dataType, string? raw)
    {
        ApplyValue(cell, dataType, new UpdateCellValueDto
        {
            TextValue = raw,
            NumberValue = decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out var n) ? n
                : decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out n) ? n : null,
            DateTimeValue = TryParseDateTime(raw ?? string.Empty, out var dt) ? dt : null,
            BooleanValue = TryParseBoolean(raw ?? string.Empty, out var b) ? b : null
        });
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
}
