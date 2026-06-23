using Quartz;

namespace CustomizableTablesWithDeadlines.Infrastructure.Scheduling;

internal static class NotificationJobKeys
{
    public const string Group = "deadline-notifications";

    public static JobKey CreateJobKey(int deadlineId, int notificationRuleId) =>
        new($"deadline-{deadlineId}-rule-{notificationRuleId}", Group);

    public static TriggerKey CreateTriggerKey(int deadlineId, int notificationRuleId) =>
        new($"trigger-deadline-{deadlineId}-rule-{notificationRuleId}", Group);
}
