using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Models;

// Represents a quality issue or non-conformance
public class Deviation : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public Job Job { get; set; } = null!;

    public Guid? MeasurementId { get; set; }

    public Measurement? Measurement { get; set; }

    public string Description { get; set; } = string.Empty;

    // Severity of the deviation, e.g., Minor, Major, Critical
    public Severity Severity { get; set; }

    // Status of the deviation, e.g., Open, Resolved, Closed
    public DeviationStatus Status { get; set; }
    = DeviationStatus.Open;

    public DateTimeOffset? ResolvedAtUtc { get; set; }

    public string? ResolvedByUserId { get; set; }
}