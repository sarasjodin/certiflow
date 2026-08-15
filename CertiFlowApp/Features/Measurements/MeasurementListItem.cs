using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Features.Measurements;

// Sealed because it is a read model used for UI (not intended to be inherited)
public sealed class MeasurementListItem
{
    // Read model for the measurements index page
    public Guid Id { get; init; }

    public string JobNumber { get; init; } = string.Empty;

    public string ToolName { get; init; } = string.Empty;

    public string ToolSerialNumber { get; init; } = string.Empty;

    public decimal Value { get; init; }

    public string Unit { get; init; } = string.Empty;

    public Guid JobId { get; init; }

    public Guid CustomerId { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public MeasurementStatus Status { get; init; }

    public DateTimeOffset MeasuredAtUtc { get; init; }
}

