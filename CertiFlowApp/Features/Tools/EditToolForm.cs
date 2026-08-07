using System.ComponentModel.DataAnnotations;

namespace CertiFlowApp.Features.Tools;

// Form model used when editing a measuring tool.
public class EditToolForm
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Tool name is required.")]
    [StringLength(150, MinimumLength = 2,
    ErrorMessage = "Tool name must contain between 2 and 150 characters.")]
    [Display(Name = "Tool name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Serial number is required.")]
    [StringLength(100,
    ErrorMessage = "Serial number cannot exceed 100 characters.")]
    [Display(Name = "Serial number")]
    public string SerialNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tool type is required.")]
    [StringLength(100,
    ErrorMessage = "Tool type cannot exceed 100 characters.")]
    [Display(Name = "Tool type")]
    public string ToolType { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Calibration valid until")]
    public DateOnly? CalibrationValidUntil { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}