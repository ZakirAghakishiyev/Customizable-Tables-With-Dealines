namespace CustomizableTablesWithDeadlines.Domain.Entities.Base;

public abstract class AuditedEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
