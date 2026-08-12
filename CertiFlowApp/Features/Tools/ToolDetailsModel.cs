namespace CertiFlowApp.Features.Tools;

public sealed class ToolDetailsModel
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string SerialNumber { get; init; } = string.Empty;

    public string ToolType { get; init; } = string.Empty;

    public DateOnly? CalibrationValidUntil { get; init; }

    public bool IsActive { get; init; }

    public int MeasurementCount { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string CreatedByUserId { get; init; } = string.Empty;

    public string CreatedByUserName { get; init; } = string.Empty;

    public DateTimeOffset? UpdatedAtUtc { get; init; }

    public string? UpdatedByUserId { get; init; }

    public string? UpdatedByUserName { get; init; }
}

