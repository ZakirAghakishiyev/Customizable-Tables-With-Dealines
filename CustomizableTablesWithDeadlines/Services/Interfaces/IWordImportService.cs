using CustomizableTablesWithDeadlines.Models;

namespace CustomizableTablesWithDeadlines.Services.Interfaces;

public interface IWordImportService
{
    IReadOnlyList<WordTablePreview> ParseTables(string filePath);

    CustomTable BuildTable(
        WordTablePreview table,
        string name,
        IReadOnlyList<int> deadlineColumnIndices);
}
