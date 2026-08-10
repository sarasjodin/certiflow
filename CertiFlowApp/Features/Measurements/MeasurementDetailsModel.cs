using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Features.Measurements;

// Sealed because it is a read model used for UI (not intended to be inherited)
public sealed class MeasurementDetailsModel
{
    // Read model for the measurement details view
    // The content in the MeasurementDetailsModel should reflect what the user wish to see in the measurement details view in the UI
    public Guid Id { get; init; }

    public string JobNumber { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string ToolName { get; init; } = string.Empty;

    public string ToolSerialNumber { get; init; } = string.Empty;

    public decimal Value { get; init; }

    public string Unit { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public MeasurementStatus Status { get; init; }

    public DateTimeOffset MeasuredAtUtc { get; init; }

    public string PerformedByUserId { get; init; } = string.Empty;

    public DateTimeOffset? VerifiedAtUtc { get; init; }

    public string? VerifiedByUserId { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string CreatedByUserId { get; init; } = string.Empty;

    public DateTimeOffset? UpdatedAtUtc { get; init; }

    public string? UpdatedByUserId { get; init; }
}
