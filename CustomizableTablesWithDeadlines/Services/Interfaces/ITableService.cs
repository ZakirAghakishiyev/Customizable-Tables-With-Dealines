using CustomizableTablesWithDeadlines.Models;

namespace CustomizableTablesWithDeadlines.Services.Interfaces;

public interface ITableService
{
    event EventHandler? TablesChanged;

    Task<IReadOnlyList<CustomTable>> GetAllTablesAsync();
    Task<CustomTable?> GetTableByIdAsync(Guid id);
    Task<CustomTable> CreateTableAsync(string name);
    Task<CustomTable> ImportTableAsync(CustomTable table);
    Task UpdateTableAsync(CustomTable table);
    Task DeleteTableAsync(Guid id);
    Task RenameTableAsync(Guid id, string newName);
}
