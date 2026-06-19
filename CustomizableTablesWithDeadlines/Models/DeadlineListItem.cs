using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Models;

public class DeadlineListItem
{
    public Guid DeadlineId { get; set; }
    public Guid TableId { get; set; }
    public Guid RowId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string RowInfo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime DeadlineDateTime { get; set; }
    public bool IsCompleted { get; set; }

    public DeadlineStatus Status => DeadlineHelper.GetStatus(DeadlineDateTime, IsCompleted);
    public TimeSpan Remaining => DeadlineDateTime - DateTime.Now;
}
