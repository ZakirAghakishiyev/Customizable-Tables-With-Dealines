using System.IO;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.DTOs.Import;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace CustomizableTablesWithDeadlines.Infrastructure.WordImport;

public class WordImportService : IWordImportService
{
    public Task<IReadOnlyList<DetectedWordTableDto>> DetectTablesAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ValidateFilePath(filePath);

        return Task.Run(() =>
        {
            var tables = ParseTables(filePath);
            IReadOnlyList<DetectedWordTableDto> result = tables
                .Select(t => new DetectedWordTableDto
                {
                    TableIndex = t.Index,
                    RowCount = t.Rows.Count,
                    ColumnCount = t.Rows.FirstOrDefault()?.Count ?? 0,
                    PreviewRows = t.Rows
                })
                .ToList();
            return result;
        }, cancellationToken);
    }

    public Task<ImportTableResultDto> ImportTableAsync(
        string filePath,
        int tableIndex,
        bool firstRowAsHeader,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ValidateFilePath(filePath);

        return Task.Run(() =>
        {
            var tables = ParseTables(filePath);
            var table = tables.FirstOrDefault(t => t.Index == tableIndex)
                        ?? throw new InvalidOperationException($"Table index {tableIndex} was not found in the document.");

            if (table.Rows.Count == 0)
                throw new InvalidOperationException("The selected table has no rows.");

            var suggestedName = Path.GetFileNameWithoutExtension(filePath);
            List<string> columnNames;
            List<List<string>> dataRows;

            if (firstRowAsHeader)
            {
                columnNames = NormalizeColumnNames(table.Rows[0]);
                dataRows = table.Rows.Skip(1).ToList();
            }
            else
            {
                var columnCount = table.Rows.Max(r => r.Count);
                columnNames = Enumerable.Range(1, columnCount).Select(i => $"Column {i}").ToList();
                dataRows = table.Rows;
            }

            return new ImportTableResultDto
            {
                SuggestedTableName = suggestedName,
                ColumnNames = columnNames,
                Rows = dataRows
            };
        }, cancellationToken);
    }

    private static void ValidateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is required.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Word document was not found.", filePath);

        if (!filePath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only .docx files are supported.");
    }

    private static List<ParsedWordTable> ParseTables(string filePath)
    {
        using var document = WordprocessingDocument.Open(filePath, false);
        var body = document.MainDocumentPart?.Document?.Body;
        if (body is null)
            return [];

        var tables = new List<ParsedWordTable>();
        var index = 0;

        foreach (var table in body.Descendants<WordTable>())
        {
            var rows = table.Elements<TableRow>()
                .Select(row => row.Elements<TableCell>()
                    .Select(GetCellText)
                    .ToList())
                .Where(row => row.Count > 0)
                .ToList();

            if (rows.Count > 0)
                tables.Add(new ParsedWordTable(index++, rows));
        }

        return tables;
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
                candidate = $"{name} ({suffix++})";

            names.Add(candidate);
        }

        return names;
    }

    private sealed record ParsedWordTable(int Index, List<List<string>> Rows);
}
