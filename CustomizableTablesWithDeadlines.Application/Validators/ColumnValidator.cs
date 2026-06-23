namespace CustomizableTablesWithDeadlines.Application.Validators;

public static class ColumnValidator
{
    public const int MaxNameLength = 100;

    public static void ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.ValidationException("Column name is required.");

        if (name.Trim().Length > MaxNameLength)
            throw new Exceptions.ValidationException($"Column name cannot exceed {MaxNameLength} characters.");
    }

    public static void EnsureUniqueName(string name, IEnumerable<string> existingNames, int? excludeColumnId = null)
    {
        var normalized = name.Trim();
        var duplicate = existingNames.Any(existing =>
            string.Equals(existing, normalized, StringComparison.OrdinalIgnoreCase));

        if (duplicate)
            throw new Exceptions.DuplicateNameException($"A column named '{normalized}' already exists in this table.");
    }
}
