namespace CustomizableTablesWithDeadlines.Application.Exceptions;

public class InvalidCellValueException : Exception
{
    public InvalidCellValueException(string message) : base(message)
    {
    }
}
