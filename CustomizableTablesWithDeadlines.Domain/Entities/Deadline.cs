using CustomizableTablesWithDeadlines.Domain.Entities.Base;

namespace CustomizableTablesWithDeadlines.Domain.Entities;

public class Deadline : AuditedEntity
{
    public int RowId { get; set; }
    public Row Row { get; set; } = null!;

    public string Title { get; set; } = null!;
    public DateTime DeadlineDateTime { get; set; }

    public ICollection<NotificationRule> NotificationRules { get; set; } = new List<NotificationRule>();
    public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
}
