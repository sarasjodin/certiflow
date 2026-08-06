using CertiFlowApp.Data;
using CertiFlowApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CertiFlowApp.Features.Tools
{
    public class ToolService
    {
        private readonly AppDbContext _dbContext;

        public ToolService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Read-only queries 
        // Returns all tools without EF Core change tracking
        // .AsNoTracking() to avoid unnecessary EF Core change tracking -> for all GetAllAsync(),GetByIdAsync() and GetEditFormAsync()
        public async Task<List<Tool>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tools
                .AsNoTracking()
                .OrderBy(tool => tool.Name)
                .ToListAsync(cancellationToken);
        }

        // Returns one tool, including its related measurements
        public async Task<Tool?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tools
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
            var name = form.Name.Trim();
            var serialNumber = form.SerialNumber.Trim();
            var toolType = form.ToolType.Trim();

            // Prevent duplicate serial numbers when new tool is created
            var serialNumberExists =
                await _dbContext.Tools.AnyAsync(
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
                CalibrationStatus = form.CalibrationStatus,
                CalibrationValidUntil = form.CalibrationValidUntil,
                IsActive = form.IsActive
            };

            _dbContext.Tools.Add(tool);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return tool;
        }

        // Returns tool data prepared for the edit form
        public async Task<EditToolForm?> GetEditFormAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tools
                .AsNoTracking()
                .Where(tool => tool.Id == id)
                .Select(tool => new EditToolForm
                {
                    Id = tool.Id,
                    Name = tool.Name,
                    SerialNumber = tool.SerialNumber,
                    ToolType = tool.ToolType,
                    CalibrationStatus = tool.CalibrationStatus,
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
            var tool = await _dbContext.Tools
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
                await _dbContext.Tools.AnyAsync(
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
            tool.CalibrationStatus = form.CalibrationStatus;
            tool.CalibrationValidUntil = form.CalibrationValidUntil;
            tool.IsActive = form.IsActive;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Prevent deletion when historical measurements exist
        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var tool = await _dbContext.Tools
                .SingleOrDefaultAsync(
                    tool => tool.Id == id,
                    cancellationToken);

            if (tool is null)
            {
                return false;
            }

            var hasMeasurements = await _dbContext.Measurements.AnyAsync(
                measurement => measurement.ToolId == id,
                cancellationToken);

            if (hasMeasurements)
            {
                throw new InvalidOperationException(
                    "The tool cannot be deleted because it has related measurements.");
            }

            _dbContext.Tools.Remove(tool);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}