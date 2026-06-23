namespace CustomizableTablesWithDeadlines.Application.Validators;

public static class DeadlineValidator
{
    public static void ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new Exceptions.ValidationException("Deadline title is required.");
    }

    public static void ValidateDateTime(DateTime deadlineDateTime)
    {
        if (deadlineDateTime == default)
            throw new Exceptions.ValidationException("Deadline date/time is required.");
    }
}
