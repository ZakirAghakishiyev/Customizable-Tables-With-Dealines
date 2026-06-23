using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Models;

public class DeadlineItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DeadlineDateTime { get; set; } = DateTime.Now.AddDays(1);
    public NotifyBeforeOption NotifyBefore { get; set; } = NotifyBeforeOption.OneHour;
    public int? CustomNotifyMinutes { get; set; }
    public bool IsCompleted { get; set; }
    public string NotifyBeforeText { get; set; } = string.Empty;
}
