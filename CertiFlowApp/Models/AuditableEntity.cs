namespace CertiFlowApp.Models;

// Base class for common audit fields
public abstract class AuditableEntity
{
    public DateTimeOffset CreatedAtUtc { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public string? UpdatedByUserId { get; set; }
}
