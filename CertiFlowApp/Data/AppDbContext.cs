using CertiFlowApp.Models;
using CertiFlowApp.Services.CurrentUser;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CertiFlowApp.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{

    // Provides access to the current authenticated user.
    private readonly ICurrentUser _currentUser;

    // Provides the current UTC time
    private readonly TimeProvider _timeProvider;

    // Receives database configuration through dependency injection
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUser currentUser,
        TimeProvider timeProvider)
        : base(options)
    {
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    // Entity sets used by EF Core to create and access database tables
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Measurement> Measurements => Set<Measurement>();
    public DbSet<Tool> Tools => Set<Tool>();
    public DbSet<Deviation> Deviations => Set<Deviation>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Applies Identity configuration
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }

    // Applies audit information before synchronous database changes are saved
    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    // Applies audit information before asynchronous database changes are saved
    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(cancellationToken);
    }

    // Automatically sets created and updated audit fields
    private void ApplyAuditInformation()
    {
        var userId = _currentUser.UserId;
        var utcNow = _timeProvider.GetUtcNow();

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            // Set creation information for new entities.
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedAtUtc == default)
                {
                    entry.Entity.CreatedAtUtc = utcNow;
                }

                if (string.IsNullOrWhiteSpace(entry.Entity.CreatedByUserId))
                {
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        throw new InvalidOperationException(
                            "CreatedByUserId must be set when no authenticated user is available.");
                    }

                    entry.Entity.CreatedByUserId = userId;
                }
            }

            // Preserve creation information and set update information
            if (entry.State == EntityState.Modified)
            {
                entry.Property(entity => entity.CreatedAtUtc).IsModified = false;
                entry.Property(entity => entity.CreatedByUserId).IsModified = false;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new InvalidOperationException(
                        "UpdatedByUserId must be set when no authenticated user is available.");
                }

                entry.Entity.UpdatedAtUtc = utcNow;
                entry.Entity.UpdatedByUserId = userId;
            }
        }
    }
}
