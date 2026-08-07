using CertiFlowApp.Models.Enums;

namespace CertiFlowApp.Features.Tools;

// Calibration status is calculated from the calibration expiry date
// Tool is valid on the expiry date - validUntil >= today
public static class ToolCalibrationRules
{
    public static CalibrationStatus GetStatus(
        DateOnly? validUntil,
        DateOnly today)
    {
        if (validUntil is null)
        {
            return CalibrationStatus.MissingDate;
        }

        return validUntil.Value >= today
            ? CalibrationStatus.Valid
            : CalibrationStatus.Expired;
    }
}