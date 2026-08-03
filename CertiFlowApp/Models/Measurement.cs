using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Models;

// Stores a measurement performed using a tool
public class Measurement : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }  // Foreign Key

    // Navigation property - whole object ref.
    public Job Job { get; set; } = null!;

    public Guid ToolId { get; set; } // Foreign Key

    // Navigation property - whole object ref.
    public Tool Tool { get; set; } = null!;

    public decimal Value { get; set; }

    public string Unit { get; set; } = string.Empty;

    public string? Notes { get; set; }

    // Status of the measurement, e.g., Draft, Submitted, Rejected
    public MeasurementStatus Status { get; set; }
    = MeasurementStatus.Draft;

    public DateTimeOffset MeasuredAtUtc { get; set; }

    public string PerformedByUserId { get; set; } = string.Empty;

    public DateTimeOffset? VerifiedAtUtc { get; set; }

    public string? VerifiedByUserId { get; set; }
}