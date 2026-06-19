using System.Globalization;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.Services;

public partial class WordImportService : IWordImportService
{
    private static readonly string[] DeadlineHeaderKeywords =
    [
        "deadline", "due", "date", "time", "expires", "expiry", "end", "finish", "target"
    ];

    public IReadOnlyList<WordTablePreview> ParseTables(string filePath)
    {
        using var document = WordprocessingDocument.Open(filePath, false);
        var body = document.MainDocumentPart?.Document?.Body;
        if (body is null)
            return [];

        var tables = new List<WordTablePreview>();
        var index = 0;

        foreach (var table in body.Descendants<Table>())
        {
            var rows = table.Elements<TableRow>()
                .Select(row => row.Elements<TableCell>()
                    .Select(GetCellText)
                    .ToList())
                .Where(row => row.Count > 0)
                .ToList();

            if (rows.Count > 0)
            {
                tables.Add(new WordTablePreview { Index = index++, Rows = rows });
            }
        }

        return tables;
    }

    public CustomTable BuildTable(
        WordTablePreview table,
        string name,
        IReadOnlyList<int> deadlineColumnIndices)
    {
        if (table.Rows.Count == 0)
            throw new InvalidOperationException("Table has no rows.");

        var headerRow = table.Rows[0];
        var columnCount = headerRow.Count;
        var columnNames = NormalizeColumnNames(headerRow);
        var dataRows = table.Rows.Skip(1).ToList();

        var columns = new List<TableColumnDefinition>();
        for (var i = 0; i < columnCount; i++)
        {
            var values = dataRows
                .Where(r => i < r.Count)
                .Select(r => r[i])
                .ToList();

            columns.Add(new TableColumnDefinition
            {
                Name = columnNames[i],
                Type = DetectColumnType(columnNames[i], values),
                Order = i
            });
        }

        var deadlineColumns = new HashSet<int>(deadlineColumnIndices);
        var rows = new List<TableRowData>();

        foreach (var dataRow in dataRows)
        {
            var row = new TableRowData();
            for (var i = 0; i < columns.Count; i++)
            {
                var rawValue = i < dataRow.Count ? dataRow[i] : string.Empty;
                row.CellValues[columns[i].Id] = ParseCellValue(rawValue, columns[i].Type);
            }

            foreach (var colIndex in deadlineColumns)
            {
                if (colIndex >= columns.Count)
                    continue;

                var column = columns[colIndex];
                var rawValue = colIndex < dataRow.Count ? dataRow[colIndex] : string.Empty;
                if (TryParseDateTime(rawValue, out var deadlineDate))
                {
                    row.Deadlines.Add(new DeadlineItem
                    {
                        Title = column.Name,
                        DeadlineDateTime = deadlineDate,
                        NotifyBefore = NotifyBeforeOption.OneDay
                    });
                }
            }

            rows.Add(row);
        }

        return new CustomTable
        {
            Name = name,
            Columns = columns,
            Rows = rows,
            LastUpdated = DateTime.Now
        };
    }

    public static IReadOnlyList<int> SuggestDeadlineColumns(
        IReadOnlyList<string> columnNames,
        IReadOnlyList<IReadOnlyList<string>> dataRows)
    {
        var suggestions = new List<int>();

        for (var i = 0; i < columnNames.Count; i++)
        {
            var values = dataRows
                .Where(r => i < r.Count)
                .Select(r => r[i])
                .ToList();

            var type = DetectColumnType(columnNames[i], values);
            if (type == ColumnType.DateTime || LooksLikeDeadlineHeader(columnNames[i]))
                suggestions.Add(i);
        }

        return suggestions;
    }

    private static string GetCellText(TableCell cell) =>
        string.Join(" ", cell.Descendants<Text>().Select(t => t.Text)).Trim();

    private static List<string> NormalizeColumnNames(IReadOnlyList<string> headers)
    {
        var names = new List<string>(headers.Count);
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < headers.Count; i++)
        {
            var name = string.IsNullOrWhiteSpace(headers[i]) ? $"Column {i + 1}" : headers[i].Trim();
            var candidate = name;
            var suffix = 2;

            while (!used.Add(candidate))
            {
                candidate = $"{name} ({suffix++})";
            }

            names.Add(candidate);
        }

        return names;
    }

    private static ColumnType DetectColumnType(string columnName, IReadOnlyList<string> values)
    {
        if (LooksLikeDeadlineHeader(columnName))
            return ColumnType.DateTime;

        var nonEmpty = values.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
        if (nonEmpty.Count == 0)
            return ColumnType.Text;

        var dateCount = nonEmpty.Count(v => TryParseDateTime(v, out _));
        var numberCount = nonEmpty.Count(v => double.TryParse(v, NumberStyles.Any, CultureInfo.CurrentCulture, out _)
                                              || double.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out _));
        var boolCount = nonEmpty.Count(IsBool);

        const double threshold = 0.6;
        if (dateCount >= nonEmpty.Count * threshold)
            return ColumnType.DateTime;
        if (numberCount >= nonEmpty.Count * threshold)
            return ColumnType.Number;
        if (boolCount >= nonEmpty.Count * threshold)
            return ColumnType.Boolean;

        return ColumnType.Text;
    }

    private static bool LooksLikeDeadlineHeader(string name)
    {
        var normalized = name.Trim().ToLowerInvariant();
        return DeadlineHeaderKeywords.Any(keyword => normalized.Contains(keyword));
    }

    private static object? ParseCellValue(string raw, ColumnType type) => type switch
    {
        ColumnType.DateTime when TryParseDateTime(raw, out var dt) => dt,
        ColumnType.Number when double.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out var n) => n,
        ColumnType.Number when double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var n) => n,
        ColumnType.Boolean when IsBool(raw, out var b) => b,
        _ => raw
    };

    private static bool TryParseDateTime(string value, out DateTime result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out result))
            return true;

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result))
            return true;

        if (DateOnlyPattern().IsMatch(value)
            && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            return true;

        return false;
    }

    private static bool IsBool(string value) => IsBool(value, out _);

    private static bool IsBool(string value, out bool result)
    {
        result = false;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Trim().ToLowerInvariant();
        switch (normalized)
        {
            case "true" or "yes" or "y" or "1":
                result = true;
                return true;
            case "false" or "no" or "n" or "0":
                return true;
            default:
                return bool.TryParse(value, out result);
        }
    }

    [GeneratedRegex(@"^\d{1,4}[-/.]\d{1,2}[-/.]\d{1,4}")]
    private static partial Regex DateOnlyPattern();
}
