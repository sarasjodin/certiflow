using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Features.Tools;

// Read model used by the tool list
// CalibrationStatus is now calculated at runtime (no longer stored in the database)
public class ToolListItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string ToolType { get; set; } = string.Empty;

    public DateOnly? CalibrationValidUntil { get; set; }

    public CalibrationStatus CalibrationStatus { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}