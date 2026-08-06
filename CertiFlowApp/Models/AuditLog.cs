using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Models;

// Stores critical business events
public class AuditLog
{
    public Guid Id { get; set; }

    public string EntityType { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;


    // The action performed on the entity (e.g., Created, Updated, Deleted)
    public AuditAction Action { get; set; }

    public DateTimeOffset PerformedAtUtc { get; set; }

    public string PerformedByUserId { get; set; } = string.Empty;

    public string? Description { get; set; }
}