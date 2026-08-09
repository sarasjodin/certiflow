using System.ComponentModel.DataAnnotations;

namespace CertiFlowApp.Features.Jobs;

// Contains only fields the user is allowed to set when creating a job
// While status, approval data and audit fields are controlled by the application
public sealed class CreateJobForm
{
    [Required(ErrorMessage = "Customer is required.")]
    [Display(Name = "Customer")]
    public Guid? CustomerId { get; set; }

    [Required(ErrorMessage = "Job number is required.")]
    [Display(Name = "Job number")]
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "Job number must be between 2 and 50 characters.")]
    public string JobNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required.")]
    [Display(Name = "Title")]
    [StringLength(
        150,
        MinimumLength = 2,
        ErrorMessage = "Title must be between 2 and 150 characters.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [StringLength(
        2000,
        ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }
}

