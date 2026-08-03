namespace CertiFlowApp.Models;

// Represents a customer owning one or more jobs
public class Customer : AuditableEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string OrganizationNumber { get; set; } = string.Empty;
    

    // Collection navigation properties - whole object references to Job objects
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}