using CertiFlowApp.Data;
using CertiFlowApp.Features.Tools;
using CertiFlowApp.Models.Enums;
using CertiFlowApp.Services.DateTime;
using Microsoft.EntityFrameworkCore;
namespace CertiFlowApp.Features.Public;

public class PublicDashboardService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly TimeProvider _timeProvider;

    public PublicDashboardService(
        IDbContextFactory<AppDbContext> dbContextFactory,
    TimeProvider timeProvider)
    {
        _dbContextFactory = dbContextFactory;
        _timeProvider = timeProvider;
    }

    // Returns statistics for public dashboard
    // Approved jobs and measurements are counted directly in the database
    // `Available tools count` needs business logic
    public async Task<PublicDashboardDto> GetPublicAsync(
        CancellationToken cancellationToken = default)
    {
        await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Get today's date in the application time zone
        // for the calibration status calculation
        var today = ApplicationDateTime.Today(_timeProvider);

        // Get only calibration status and active state tool properties
        var tools = await db.Tools
            .AsNoTracking()
            .Select(tool => new
            {
                tool.CalibrationValidUntil,
                tool.IsActive
            })
            // To a temporary list
            .ToListAsync(cancellationToken);

        // Count the number of approved jobs directly in the database
        var approvedJobCount = await db.Jobs.CountAsync(
            job => job.Status == JobStatus.Approved,
            cancellationToken);

        // Count the number of measurements directly in the database
        var measurementCount = await db.Measurements.CountAsync(
            cancellationToken);

        // A tool is available when it is active
        // and its calibration is still valid
        var availableToolCount = tools.Count(tool =>
            tool.IsActive &&
            ToolCalibrationRules.GetStatus(
                tool.CalibrationValidUntil,
                today) == CalibrationStatus.Valid);

        // Create and return the public dashboard DTO
        return new PublicDashboardDto
        {
            ApprovedJobCount = approvedJobCount,
            MeasurementCount = measurementCount,
            AvailableToolCount = availableToolCount
        };
    }
}
