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
                    Status = job.Status,
                    MeasurementCount = job.Measurements.Count
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
                    ApprovedByUserName = job.ApprovedByUserId == null
                        ? null
                        : db.Users
                            .Where(user => user.Id == job.ApprovedByUserId)
                            .Select(user => user.UserName)
                            .SingleOrDefault(),
                    CreatedAtUtc = job.CreatedAtUtc,
                    CreatedByUserId = job.CreatedByUserId,
                    CreatedByUserName = db.Users
                        .Where(user => user.Id == job.CreatedByUserId)
                        .Select(user => user.UserName)
                        .SingleOrDefault()
                        ?? job.CreatedByUserId,
                    UpdatedAtUtc = job.UpdatedAtUtc,
                    UpdatedByUserId = job.UpdatedByUserId,
                    UpdatedByUserName = job.UpdatedByUserId == null
                        ? null
                        : db.Users
                            .Where(user => user.Id == job.UpdatedByUserId)
                            .Select(user => user.UserName)
                            .SingleOrDefault(),

                    // Get the count of related measurement and deviations to this job
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

            // Check if jobNumber already exists
            var jobNumberExists = await db.Jobs.AnyAsync(
                job => job.JobNumber == jobNumber,
                cancellationToken);

            if (jobNumberExists)
            {
                throw new InvalidOperationException(
                    "A job with this job number already exists.");
            }

            // CustomerId is nullable in the form but a Job must always have a customer
            if (!form.CustomerId.HasValue)
            {
                throw new InvalidOperationException(
                    "A customer must be selected.");
            }

            var customerId = form.CustomerId.Value;

            // Check if the customer exists before creating the job
            var customerExists = await db.Customers.AnyAsync(
                customer => customer.Id == customerId,
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
                CustomerId = customerId,
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
                    CustomerName = job.Customer.Name,
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
