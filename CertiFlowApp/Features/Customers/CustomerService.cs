using CertiFlowApp.Data;
using CertiFlowApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CertiFlowApp.Features.Customers
{
    // Handles database operations for customers as a Service instead of adding database logic directly to Blazor pages
    // All asynchronous operations support CancellationToken to allow graceful request cancellation
    public class CustomerService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public CustomerService(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // Read-only queries 
        // Returns all customers without EF Core change tracking
        // .AsNoTracking() to avoid unnecessary EF Core change tracking -> for all GetAllAsync(),GetByIdAsync() and GetEditFormAsync()
        public async Task<List<Customer>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            return await db.Customers
                .AsNoTracking()
                .OrderBy(customer => customer.Name)
                .ToListAsync(cancellationToken);
        }

        // Returns customer data prepared for the details view
        public async Task<CustomerDetailsModel?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            return await db.Customers
                .AsNoTracking()
                .Where(customer => customer.Id == id)
                .Select(customer => new CustomerDetailsModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    OrganizationNumber = customer.OrganizationNumber,

                    CreatedAtUtc = customer.CreatedAtUtc,
                    CreatedByUserId = customer.CreatedByUserId,
                    CreatedByUserName = db.Users
                        .Where(user => user.Id == customer.CreatedByUserId)
                        .Select(user => user.UserName)
                        .SingleOrDefault()
                        ?? customer.CreatedByUserId,

                    UpdatedAtUtc = customer.UpdatedAtUtc,
                    UpdatedByUserId = customer.UpdatedByUserId,
                    UpdatedByUserName = customer.UpdatedByUserId == null
                        ? null
                        : db.Users
                            .Where(user => user.Id == customer.UpdatedByUserId)
                            .Select(user => user.UserName)
                            .SingleOrDefault(),

                    JobCount = customer.Jobs.Count
                })
                .SingleOrDefaultAsync(cancellationToken);
        }

        // Creates a new customer.
        public async Task<Customer> CreateAsync(
            CreateCustomerForm form,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var name = form.Name.Trim();

            var organizationNumber = form.OrganizationNumber.Trim();

            var organizationNumberExists =
                await db.Customers.AnyAsync(
                    customer =>
                        customer.OrganizationNumber == organizationNumber,
                    cancellationToken);

            if (organizationNumberExists)
            {
                throw new InvalidOperationException(
                    "A customer with this organization number already exists.");
            }

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = name,
                OrganizationNumber = organizationNumber
            };

            db.Customers.Add(customer);
            await db.SaveChangesAsync(cancellationToken);

            return customer;
        }

        // Returns customer data prepared for the edit form
        public async Task<EditCustomerForm?> GetEditFormAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            return await db.Customers
                .AsNoTracking()
                .Where(customer => customer.Id == id)
                .Select(customer => new EditCustomerForm
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    OrganizationNumber = customer.OrganizationNumber
                })
                .SingleOrDefaultAsync(cancellationToken);
        }

        // Updates an existing customer
        public async Task<bool> UpdateAsync(
            EditCustomerForm form,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var customer = await db.Customers
                .SingleOrDefaultAsync(
                    customer => customer.Id == form.Id,
                    cancellationToken);

            if (customer is null)
            {
                return false;
            }

            var name = form.Name.Trim();

            var organizationNumber = form.OrganizationNumber.Trim();

            // Check if another customer with the same organization number exists
            var organizationNumberExists =
                await db.Customers.AnyAsync(
                    otherCustomer =>
                        otherCustomer.Id != form.Id &&
                        otherCustomer.OrganizationNumber == organizationNumber,
                    cancellationToken);

            if (organizationNumberExists)
            {
                throw new InvalidOperationException(
                    "A customer with this organization number already exists.");
            }

            customer.Name = name;
            customer.OrganizationNumber = organizationNumber;

            await db.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Deletes a customer only when it has no jobs.
        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var db =
                await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var customer = await db.Customers
                .SingleOrDefaultAsync(
                    customer => customer.Id == id,
                    cancellationToken);

            if (customer is null)
            {
                return false;
            }

            var hasJobs = await db.Jobs.AnyAsync(
                job => job.CustomerId == id,
                cancellationToken);

            if (hasJobs)
            {
                throw new InvalidOperationException(
                    "The customer cannot be deleted because it has related jobs.");
            }

            db.Customers.Remove(customer);
            await db.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
