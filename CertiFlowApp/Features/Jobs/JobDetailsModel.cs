using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Features.Jobs;

// Sealed because it is a read selection model used for UI (not intended to be inherited)
public sealed class JobDetailsModel
{
    // Read model for the job details view
    // The content in the JobDetailsModel should reflect what the user wish to see in the job details view in the UI
    public Guid Id { get; init; }

    public string JobNumber { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public JobStatus Status { get; init; }

    public string? CertificateNumber { get; init; }

    public DateTimeOffset? ApprovedAtUtc { get; init; }

    public string? ApprovedByUserId { get; init; }

    public string? ApprovedByUserName { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string CreatedByUserId { get; init; } = string.Empty;

    public string CreatedByUserName { get; init; } = string.Empty;

    public DateTimeOffset? UpdatedAtUtc { get; init; }

    public string? UpdatedByUserId { get; init; }

    public string? UpdatedByUserName { get; init; }


    public int MeasurementCount { get; init; }

    public int DeviationCount { get; init; }
}
