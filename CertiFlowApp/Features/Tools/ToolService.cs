using CertiFlowApp.Data;
using CertiFlowApp.Models;
using CertiFlowApp.Services.DateTime;
using Microsoft.EntityFrameworkCore;

namespace CertiFlowApp.Features.Tools
{
    public class ToolService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly TimeProvider _timeProvider;

        public ToolService(
            IDbContextFactory<AppDbContext> dbContextFactory,
            TimeProvider timeProvider)
        {
            _dbContextFactory = dbContextFactory;

            // ToolService depends on TimeProvider to get the current date
            // for calibration status calculations
            _timeProvider = timeProvider;
        }

        // Read-only queries 
        // Returns all tools without EF Core change tracking
        // .AsNoTracking() to avoid unnecessary EF Core change tracking -> for all GetAllAsync(),GetByIdAsync() and GetEditFormAsync()
        public async Task<List<ToolListItem>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var tools = await db.Tools
                .AsNoTracking()
                .OrderBy(tool => tool.Name)
                .ToListAsync(cancellationToken);

            var today = ApplicationDateTime.Today(_timeProvider);

            return tools
                .Select(tool => new ToolListItem
                {
                    Id = tool.Id,
                    Name = tool.Name,
                    SerialNumber = tool.SerialNumber,
                    ToolType = tool.ToolType,
                    CalibrationValidUntil = tool.CalibrationValidUntil,
                    CalibrationStatus = ToolCalibrationRules.GetStatus(
                        tool.CalibrationValidUntil,
                        today),
                    IsActive = tool.IsActive,
                    CreatedAtUtc = tool.CreatedAtUtc
                })
                .ToList();
        }

        // Returns one tool, including its related measurements
        // TODO: Implement ToolDetailsModel
        public async Task<Tool?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            return await db.Tools
                .AsNoTracking()
                .Include(tool => tool.Measurements)
                .SingleOrDefaultAsync(
                    tool => tool.Id == id,
                    cancellationToken);
        }

        // Creates a new tool.
        public async Task<Tool> CreateAsync(
            CreateToolForm form,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var name = form.Name.Trim();
            var serialNumber = form.SerialNumber.Trim();
            var toolType = form.ToolType.Trim();

            // Prevent duplicate serial numbers when new tool is created
            var serialNumberExists =
                await db.Tools.AnyAsync(
                    tool =>
                        tool.SerialNumber == serialNumber,
                    cancellationToken);

            if (serialNumberExists)
            {
                throw new InvalidOperationException(
                    "A tool with this serial number already exists.");
            }

            var tool = new Tool
            {
                Id = Guid.NewGuid(),
                Name = name,
                SerialNumber = serialNumber,
                ToolType = toolType,
                CalibrationValidUntil = form.CalibrationValidUntil,
                IsActive = form.IsActive
            };

            db.Tools.Add(tool);
            await db.SaveChangesAsync(cancellationToken);

            return tool;
        }

        // Returns tool data prepared for the edit form
        public async Task<EditToolForm?> GetEditFormAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            return await db.Tools
                .AsNoTracking()
                .Where(tool => tool.Id == id)
                .Select(tool => new EditToolForm
                {
                    Id = tool.Id,
                    Name = tool.Name,
                    SerialNumber = tool.SerialNumber,
                    ToolType = tool.ToolType,
                    CalibrationValidUntil = tool.CalibrationValidUntil,
                    IsActive = tool.IsActive
                })
                .SingleOrDefaultAsync(cancellationToken);
        }

        // Updates an existing tool
        public async Task<bool> UpdateAsync(
            EditToolForm form,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var tool = await db.Tools
                .SingleOrDefaultAsync(
                    tool => tool.Id == form.Id,
                    cancellationToken);

            if (tool is null)
            {
                return false;
            }

            var name = form.Name.Trim();
            var serialNumber = form.SerialNumber.Trim();
            var toolType = form.ToolType.Trim();

            // Prevent duplicate serial numbers when updating a tool
            var serialNumberExists =
                await db.Tools.AnyAsync(
                    otherTool =>
                        // Filter out the current tool being updated, to check if other tool already uses the same serial number
                        otherTool.Id != form.Id &&
                        otherTool.SerialNumber == serialNumber,
                    cancellationToken);

            if (serialNumberExists)
            {
                throw new InvalidOperationException(
                    "A tool with this serial number already exists.");
            }

            tool.Name = name;
            tool.SerialNumber = serialNumber;
            tool.ToolType = toolType;
            tool.CalibrationValidUntil = form.CalibrationValidUntil;
            tool.IsActive = form.IsActive;

            await db.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Prevent deletion when historical measurements exist
        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var tool = await db.Tools
                .SingleOrDefaultAsync(
                    tool => tool.Id == id,
                    cancellationToken);

            if (tool is null)
            {
                return false;
            }

            var hasMeasurements = await db.Measurements.AnyAsync(
                measurement => measurement.ToolId == id,
                cancellationToken);

            if (hasMeasurements)
            {
                throw new InvalidOperationException(
                    "The tool cannot be deleted because it has related measurements.");
            }

            db.Tools.Remove(tool);
            await db.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Return tools as dropdown
        public async Task<List<ToolOption>> GetOptionsAsync(
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            return await db.Tools
                .AsNoTracking()
                .OrderBy(tool => tool.Name)
                .ThenBy(tool => tool.SerialNumber)
                .Select(tool => new ToolOption
                {
                    Id = tool.Id,
                    Name = tool.Name,
                    SerialNumber = tool.SerialNumber
                })
                .ToListAsync(cancellationToken);
        }
    }
}