using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Models;

// Represents a quality assurance job
public class Job : AuditableEntity
{
    public Guid Id { get; set; }

    // Customer owning this job
    public Guid CustomerId { get; set; } // Foreign key

    public Customer Customer { get; set; } = null!; // Navigation property - whole object ref.

    public string JobNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Status of the job, e.g., Draft, InProgress, Approved
    public JobStatus Status { get; set; } = JobStatus.Draft;

    public string? CertificateNumber { get; set; }

    public DateTimeOffset? ApprovedAtUtc { get; set; }

    public string? ApprovedByUserId { get; set; }


    // Measurements registered for this job
    // Collection navigation properties - whole object references to Measurement objects
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

    // Deviations registered for this job
    // Collection navigation properties - whole object references to Deviation objects
    public ICollection<Deviation> Deviations { get; set; } = new List<Deviation>();
}
