using System.ComponentModel.DataAnnotations;

namespace CertiFlowApp.Features.Measurements;

// Sealed because it is a form input model used for UI (not intended to be inherited)
public sealed class CreateMeasurementForm
{
    // Form model containing only fields the user can set when creating a measurement

    [Required(ErrorMessage = "Job is required.")]
    [Display(Name = "Job")]
    public Guid? JobId { get; set; }

    [Required(ErrorMessage = "Tool is required.")]
    [Display(Name = "Tool")]
    public Guid? ToolId { get; set; }

    [Required(ErrorMessage = "Value is required.")]
    [Display(Name = "Value")]
    // decimal? is used to allow null values (= return validation errors)
    // and 0 as a valid measurement value
    public decimal? Value { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    [Display(Name = "Unit")]
    [StringLength(
     30,
     ErrorMessage = "Unit cannot exceed 30 characters.")]
    public string Unit { get; set; } = string.Empty;

    [Display(Name = "Notes")]
    [StringLength(
        2000,
        ErrorMessage = "Notes cannot exceed 2000 characters.")]
    public string? Notes { get; set; }
}

