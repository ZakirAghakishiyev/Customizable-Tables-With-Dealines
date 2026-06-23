using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Models;

public class DeadlineListItem
{
    public int DeadlineId { get; set; }
    public int TableId { get; set; }
    public int RowId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string RowInfo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime DeadlineDateTime { get; set; }
    public bool IsCompleted { get; set; }

    public DeadlineStatus Status => DeadlineHelper.GetStatus(DeadlineDateTime, IsCompleted);
    public TimeSpan Remaining => DeadlineDateTime - DateTime.Now;
}
