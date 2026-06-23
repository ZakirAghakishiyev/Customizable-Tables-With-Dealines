namespace CustomizableTablesWithDeadlines.Application.Validators;

public static class TableValidator
{
    public const int MaxNameLength = 100;

    public static void ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.ValidationException("Table name is required.");

        if (name.Trim().Length > MaxNameLength)
            throw new Exceptions.ValidationException($"Table name cannot exceed {MaxNameLength} characters.");
    }
}
