using CertiFlowApp.Data;
using CertiFlowApp.Features.Tools;
using CertiFlowApp.Models;
using CertiFlowApp.Models.Enums;
using CertiFlowApp.Services.CurrentUser;
using CertiFlowApp.Services.DateTime;
using Microsoft.EntityFrameworkCore;

namespace CertiFlowApp.Features.Measurements;

public class MeasurementService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ICurrentUser _currentUser;

    public MeasurementService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        TimeProvider timeProvider,
        ICurrentUser currentUser)
    {
        _dbContextFactory = dbContextFactory;

        // MeasurementService depends on TimeProvider to set MeasuredAtUtc
        // using the system time instead of user input
        _timeProvider = timeProvider;

        // MeasurementService depends on ICurrentUser to set PerformedByUserId
        _currentUser = currentUser;
    }

    // Read-only queries 
    // Returns all measurements without EF Core change tracking
    // .AsNoTracking() to avoid unnecessary EF Core change tracking -> for all GetAllAsync(),GetByIdAsync() and GetEditFormAsync()
    public async Task<List<MeasurementListItem>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Measurements
            .AsNoTracking()
            // View latest measurements first
            .OrderByDescending(measurement => measurement.MeasuredAtUtc)
            .Select(measurement => new MeasurementListItem
            // Tell the database to only select the properties needed for the MeasurementListItem,
            // instead of loading the entire Measurement entity, which makes the query faster.
            {
                Id = measurement.Id,
                JobId = measurement.JobId,
                JobNumber = measurement.Job.JobNumber,
                ToolName = measurement.Tool.Name,
                ToolSerialNumber = measurement.Tool.SerialNumber,
                Value = measurement.Value,
                Unit = measurement.Unit,
                Status = measurement.Status,
                MeasuredAtUtc = measurement.MeasuredAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    // Returns the measurement data needed for the details view,
    // and chosen related job and tool data
    public async Task<MeasurementDetailsModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Measurements
            .AsNoTracking()
            .Where(measurement => measurement.Id == id)
            .Select(measurement => new MeasurementDetailsModel
            {
                Id = measurement.Id,
                CustomerName = measurement.Job.Customer.Name,
                JobNumber = measurement.Job.JobNumber,
                ToolName = measurement.Tool.Name,
                ToolSerialNumber = measurement.Tool.SerialNumber,
                Value = measurement.Value,
                Unit = measurement.Unit,
                Notes = measurement.Notes,
                Status = measurement.Status,
                MeasuredAtUtc = measurement.MeasuredAtUtc,
                PerformedByUserId = measurement.PerformedByUserId,
                PerformedByUserName = db.Users
                    .Where(user => user.Id == measurement.PerformedByUserId)
                    .Select(user => user.UserName)
                    .SingleOrDefault()
                    ?? measurement.PerformedByUserId,
                VerifiedAtUtc = measurement.VerifiedAtUtc,
                VerifiedByUserId = measurement.VerifiedByUserId,
                VerifiedByUserName = measurement.VerifiedByUserId == null
                    ? null
                    : db.Users
                        .Where(user => user.Id == measurement.VerifiedByUserId)
                        .Select(user => user.UserName)
                        .SingleOrDefault(),
                CreatedAtUtc = measurement.CreatedAtUtc,
                CreatedByUserId = measurement.CreatedByUserId,

                CreatedByUserName = db.Users
                    .Where(user => user.Id == measurement.CreatedByUserId)
                    .Select(user => user.UserName)
                    .SingleOrDefault()
                    ?? measurement.CreatedByUserId,

                UpdatedAtUtc = measurement.UpdatedAtUtc,
                UpdatedByUserId = measurement.UpdatedByUserId,

                UpdatedByUserName = measurement.UpdatedByUserId == null
                    ? null
                    : db.Users
                        .Where(user => user.Id == measurement.UpdatedByUserId)
                        .Select(user => user.UserName)
                        .SingleOrDefault(),
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<List<MeasurementListItem>> GetByJobIdAsync(
    Guid jobId,
    CancellationToken cancellationToken = default)
    {
        await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Measurements
            .AsNoTracking()
            .Where(measurement => measurement.JobId == jobId)
            .OrderByDescending(measurement => measurement.MeasuredAtUtc)
            .Select(measurement => new MeasurementListItem
            {
                Id = measurement.Id,
                JobId = measurement.JobId,
                JobNumber = measurement.Job.JobNumber,
                ToolName = measurement.Tool.Name,
                ToolSerialNumber = measurement.Tool.SerialNumber,
                Value = measurement.Value,
                Unit = measurement.Unit,
                Status = measurement.Status,
                MeasuredAtUtc = measurement.MeasuredAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MeasurementListItem>> GetByToolIdAsync(
        Guid toolId,
        CancellationToken cancellationToken = default)
    {
        await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Measurements
            .AsNoTracking()
            .Where(measurement => measurement.ToolId == toolId)
            .OrderByDescending(measurement => measurement.MeasuredAtUtc)
            .Select(measurement => new MeasurementListItem
            {
                Id = measurement.Id,
                JobId = measurement.JobId,
                JobNumber = measurement.Job.JobNumber,
                CustomerId = measurement.Job.CustomerId,
                CustomerName = measurement.Job.Customer.Name,
                ToolName = measurement.Tool.Name,
                ToolSerialNumber = measurement.Tool.SerialNumber,
                Value = measurement.Value,
                Unit = measurement.Unit,
                Status = measurement.Status,
                MeasuredAtUtc = measurement.MeasuredAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    // Creates a new measurement
    public async Task<Measurement> CreateAsync(
        CreateMeasurementForm form,
        CancellationToken cancellationToken = default)
    {
        await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Check that job is selected
        if (!form.JobId.HasValue)
        {
            throw new InvalidOperationException(
                "A job must be selected.");
        }

        var jobExists = await db.Jobs.AnyAsync(
            job => job.Id == form.JobId.Value,
            cancellationToken);

        if (!jobExists)
        {
            throw new InvalidOperationException(
                "The selected job does not exist.");
        }

        // Check that tool is selected
        if (!form.ToolId.HasValue)
        {
            throw new InvalidOperationException(
                "A tool must be selected.");
        }

        var tool = await db.Tools
            .AsNoTracking()
            .SingleOrDefaultAsync(
                tool => tool.Id == form.ToolId.Value,
                cancellationToken);

        if (tool is null)
        {
            throw new InvalidOperationException(
                "The selected tool does not exist.");
        }

        var today = ApplicationDateTime.Today(_timeProvider);

        if (!tool.IsActive ||
            ToolCalibrationRules.GetStatus(
                tool.CalibrationValidUntil,
                today) != CalibrationStatus.Valid)
        {
            throw new InvalidOperationException(
                "The selected tool is not available for measurement.");
        }

        // Check that measurement value is provided
        if (!form.Value.HasValue)
        {
            throw new InvalidOperationException(
                "A measurement value is required.");
        }

        // Check that measurement unit is provided
        if (string.IsNullOrWhiteSpace(form.Unit))
        {
            throw new InvalidOperationException(
                "A measurement unit is required.");
        }

        // Store UserId in a local variable first so the same value is used
        // for both validation and the measurement
        var performedByUserId = _currentUser.UserId;

        // Check if current user is authenticated and valid
        if (!_currentUser.IsAuthenticated ||
            string.IsNullOrWhiteSpace(performedByUserId))
        {
            throw new InvalidOperationException(
                "An authenticated user is required to create a measurement.");
        }

        var unit = form.Unit.Trim();
        var notes = form.Notes?.Trim();


        var measurement = new Measurement
        {

            Id = Guid.NewGuid(),
            // Selected job and tool IDs from the form
            JobId = form.JobId.Value,
            ToolId = form.ToolId.Value,
            // Entered measurement data from the form
            Value = form.Value.Value,
            Unit = unit,
            Notes = notes,
            // System values
            Status = MeasurementStatus.Draft,
            MeasuredAtUtc = _timeProvider.GetUtcNow(),
            PerformedByUserId = performedByUserId
        };

        db.Measurements.Add(measurement);
        await db.SaveChangesAsync(cancellationToken);

        return measurement;
    }

    // Returns measurement data prepared for the edit form
    public async Task<EditMeasurementForm?> GetEditFormAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Measurements
            .AsNoTracking()
            .Where(measurement => measurement.Id == id)
            .Select(measurement => new EditMeasurementForm
            {
                Id = measurement.Id,
                Value = measurement.Value,
                Unit = measurement.Unit,
                Notes = measurement.Notes
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    // Updates an existing measurement
    public async Task<bool> UpdateAsync(
        EditMeasurementForm form,
        CancellationToken cancellationToken = default)
    {
        await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Load the measurement
        var measurement = await db.Measurements
            .SingleOrDefaultAsync(
                measurement => measurement.Id == form.Id,
                cancellationToken);

        // If the measurement does not exist, return false
        if (measurement is null)
        {
            return false;
        }

        if (!form.Value.HasValue)
        {
            throw new InvalidOperationException(
                "A measurement value is required.");
        }

        if (string.IsNullOrWhiteSpace(form.Unit))
        {
            throw new InvalidOperationException(
                "A measurement unit is required.");
        }

        var unit = form.Unit.Trim();
        var notes = form.Notes?.Trim();

        // Update fields the user is allowed to edit
        measurement.Value = form.Value.Value;
        measurement.Unit = unit;
        measurement.Notes = notes;

        await db.SaveChangesAsync(cancellationToken);

        return true;
    }

    // Prevent deletion when related deviations exist
    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var measurement = await db.Measurements
            .SingleOrDefaultAsync(
                measurement => measurement.Id == id,
                cancellationToken);

        if (measurement is null)
        {
            return false;
        }

        var hasDeviations = await db.Deviations.AnyAsync(
            deviation => deviation.MeasurementId == id,
            cancellationToken);

        if (hasDeviations)
        {
            throw new InvalidOperationException(
                "The measurement cannot be deleted because it has related deviations.");
        }

        db.Measurements.Remove(measurement);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }
}

