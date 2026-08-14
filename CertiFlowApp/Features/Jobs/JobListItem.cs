using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Features.Jobs;

// Sealed because it is a read model used for UI (not intended to be inherited)
public sealed class JobListItem
{
    // Read model for the jobs index page
    // Content should reflect what the user wish to see on Index page
    public Guid Id { get; init; }

    // Not yet decided format of job number = using string to start with
    public string JobNumber { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public JobStatus Status { get; init; }

    public int MeasurementCount { get; init; }

    // CustomerJobs table - This property is used to filter jobs by customer.
    public Guid CustomerId { get; init; }
}
