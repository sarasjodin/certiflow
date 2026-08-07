namespace CertiFlowApp.Models;

// Represents a measuring instrument
public class Tool : AuditableEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string ToolType { get; set; } = string.Empty;

    // Last calendar date on which the calibration is valid
    public DateOnly? CalibrationValidUntil { get; set; }

    // Indicates if tool can be used
    public bool IsActive { get; set; } = true;


    // Navigation property to measurements performed using this tool
    public ICollection<Measurement> Measurements { get; set; }
        = new List<Measurement>();
}
