using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Models;

public static class DeadlineHelper
{
  private static readonly TimeSpan DueSoonThreshold = TimeSpan.FromHours(24);

  public static DeadlineStatus GetStatus(DateTime deadlineDateTime, bool isCompleted)
  {
    if (isCompleted)
      return DeadlineStatus.Completed;

    var remaining = deadlineDateTime - DateTime.Now;
    if (remaining < TimeSpan.Zero)
      return DeadlineStatus.Overdue;
    if (remaining <= DueSoonThreshold)
      return DeadlineStatus.DueSoon;

    return DeadlineStatus.Upcoming;
  }

  public static int GetNotifyMinutes(NotifyBeforeOption option, int? customMinutes)
  {
    return option switch
    {
      NotifyBeforeOption.TenMinutes => 10,
      NotifyBeforeOption.ThirtyMinutes => 30,
      NotifyBeforeOption.OneHour => 60,
      NotifyBeforeOption.ThreeHours => 180,
      NotifyBeforeOption.OneDay => 1440,
      NotifyBeforeOption.Custom => customMinutes ?? 30,
      _ => 60
    };
  }

  public static string FormatRemainingTime(TimeSpan remaining, bool isCompleted)
  {
    if (isCompleted)
      return "—";

    if (remaining < TimeSpan.Zero)
    {
      var overdue = remaining.Negate();
      return overdue.TotalDays >= 1
        ? $"-{(int)overdue.TotalDays}d {overdue.Hours}h"
        : overdue.TotalHours >= 1
          ? $"-{(int)overdue.TotalHours}h {overdue.Minutes}m"
          : $"-{overdue.Minutes}m";
    }

    return remaining.TotalDays >= 1
      ? $"{(int)remaining.TotalDays}d {remaining.Hours}h"
      : remaining.TotalHours >= 1
        ? $"{(int)remaining.TotalHours}h {remaining.Minutes}m"
        : $"{remaining.Minutes}m";
  }
}
