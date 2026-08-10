using CertiFlowApp.Data;
using CertiFlowApp.Models;
using CertiFlowApp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CertiFlowApp.Features.Jobs
{
    public class JobService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public JobService(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<JobListItem>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            return await db.Jobs
                .AsNoTracking()
                .OrderBy(job => job.JobNumber)
                .Select(job => new JobListItem
                // Tell the database to only select the properties needed for the JobListItem,
                // instead of loading the entire Job entity, which makes the query faster.
                {
                    Id = job.Id,
                    JobNumber = job.JobNumber,
                    Title = job.Title,
                    CustomerName = job.Customer.Name,
                    Status = job.Status
                })

            .ToListAsync(cancellationToken);
        }

        // Returns data for the job details view including counts of related measurements and deviations
        public async Task<JobDetailsModel?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            // Load the job data for the details view
            return await db.Jobs
                .AsNoTracking()
                .Where(job => job.Id == id)
                .Select(job => new JobDetailsModel
                {
                    Id = job.Id,
                    JobNumber = job.JobNumber,
                    Title = job.Title,
                    Description = job.Description,
                    CustomerName = job.Customer.Name,
                    Status = job.Status,
                    CertificateNumber = job.CertificateNumber,
                    ApprovedAtUtc = job.ApprovedAtUtc,
                    ApprovedByUserId = job.ApprovedByUserId,
                    CreatedAtUtc = job.CreatedAtUtc,
                    CreatedByUserId = job.CreatedByUserId,
                    UpdatedAtUtc = job.UpdatedAtUtc,
                    UpdatedByUserId = job.UpdatedByUserId,
                    MeasurementCount = job.Measurements.Count,
                    DeviationCount = job.Deviations.Count
                })
            // Expected outcome of Details view =
            // A job ID should identify max one job
            // Returns null when the requested job (id) does not exist
            // SingleOrDefaultAsync expects one or zero result, otherwise throws exception
            .SingleOrDefaultAsync(cancellationToken);
        }

        // Creates a new job.
        public async Task<Job> CreateAsync(
            CreateJobForm form,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var jobNumber = form.JobNumber.Trim();
            var title = form.Title.Trim();
            var description = form.Description?.Trim();

            // Check if a customer is selected
            if (!form.CustomerId.HasValue)
            {
                throw new InvalidOperationException(
                    "A customer must be selected.");
            }

            // Check if jobNumber already exists
            var jobNumberExists = await db.Jobs.AnyAsync(
                job => job.JobNumber == jobNumber,
                cancellationToken);

            if (jobNumberExists)
            {
                throw new InvalidOperationException(
                    "A job with this job number already exists.");
            }

            // Check if the customer exists before creating the job
            var customerExists = await db.Customers.AnyAsync(
                customer => customer.Id == form.CustomerId,
                cancellationToken);

            if (!customerExists)
            {
                throw new InvalidOperationException(
                    "The selected customer does not exist.");
            }

            // Create a new job entity and set its properties
            var job = new Job
            {
                Id = Guid.NewGuid(),
                CustomerId = form.CustomerId.Value,
                JobNumber = jobNumber,
                Title = title,
                Description = description,
                // New jobs always start with status "Draft",
                // since the user should not be able to create jobs that are already in progress or approved
                Status = JobStatus.Draft
            };

            db.Jobs.Add(job);

            await db.SaveChangesAsync(cancellationToken);

            return job;
        }

        // Returns job data for the edit form
        public async Task<EditJobForm?> GetEditFormAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            // Load the job data for the edit form
            return await db.Jobs
                .AsNoTracking()
                .Where(job => job.Id == id)
                .Select(job => new EditJobForm
                {
                    Id = job.Id,
                    CustomerId = job.CustomerId,
                    JobNumber = job.JobNumber,
                    Title = job.Title,
                    Description = job.Description
                })
                .SingleOrDefaultAsync(cancellationToken);
        }

        // Updates an existing job
        public async Task<bool> UpdateAsync(
            EditJobForm form,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            // Check if a customer is selected
            if (!form.CustomerId.HasValue)
            {
                throw new InvalidOperationException(
                    "A customer must be selected.");
            }

            // Check if jobNumber exists for another job
            var jobNumberExists = await db.Jobs.AnyAsync(
                otherJob =>
                    otherJob.Id != form.Id &&
                    otherJob.JobNumber == form.JobNumber.Trim(),
                cancellationToken);

            if (jobNumberExists)
            {
                throw new InvalidOperationException(
                    "A job with this job number already exists.");
            }

            // Check if the customer exists before updating the job
            var customerExists = await db.Customers.AnyAsync(
               customer => customer.Id == form.CustomerId,
               cancellationToken);

            if (!customerExists)
            {
                throw new InvalidOperationException(
                    "The selected customer does not exist.");
            }

            // Load the existing job
            // EF Core can then track and update it
            var job = await db.Jobs
                .SingleOrDefaultAsync(
                    job => job.Id == form.Id,
                    cancellationToken);

            if (job is null)
            {
                return false;
            }

            job.CustomerId = form.CustomerId.Value;
            job.JobNumber = form.JobNumber.Trim();
            job.Title = form.Title.Trim();
            job.Description = form.Description?.Trim();

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

            // Load the existing job
            // EF Core can then track and delete it
            var job = await db.Jobs
                .SingleOrDefaultAsync(
                    job => job.Id == id,
                    cancellationToken);

            if (job is null)
            {
                return false;
            }

            // Check if the job has related measurements or deviations
            // If yes = prevent deletion to keep historical data
            var hasMeasurements = await db.Measurements.AnyAsync(
                measurement => measurement.JobId == id,
                cancellationToken);

            var hasDeviations = await db.Deviations.AnyAsync(
                deviation => deviation.JobId == id,
                cancellationToken);

            if (hasMeasurements || hasDeviations)
            {
                throw new InvalidOperationException(
                    "The job cannot be deleted because it has related measurements or deviations.");
            }

            db.Jobs.Remove(job);
            await db.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Return jobs as dropdown
        public async Task<List<JobOption>> GetOptionsAsync(
            CancellationToken cancellationToken = default)
        {
            await using var db =
            await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            return await db.Jobs
                .AsNoTracking()
                .OrderBy(job => job.JobNumber)
                .Select(job => new JobOption
                {
                    Id = job.Id,
                    JobNumber = job.JobNumber,
                    Title = job.Title
                })
                .ToListAsync(cancellationToken);
        }
    }
}
