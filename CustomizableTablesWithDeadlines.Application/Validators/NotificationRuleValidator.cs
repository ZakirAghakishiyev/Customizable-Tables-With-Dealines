namespace CustomizableTablesWithDeadlines.Application.Validators;

public static class NotificationRuleValidator
{
    public static void ValidateNotifyBeforeMinutes(int minutes)
    {
        if (minutes <= 0)
            throw new Exceptions.ValidationException("Notify before minutes must be a positive number.");
    }
}
