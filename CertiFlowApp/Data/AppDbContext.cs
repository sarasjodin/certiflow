using CertiFlowApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CertiFlowApp.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    // Receives database configuration through dependency injection
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
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
        builder.HasDefaultSchema("certiflowapp");

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}