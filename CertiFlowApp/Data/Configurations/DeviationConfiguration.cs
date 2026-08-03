using CertiFlowApp.Models;
using CertiFlowApp.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CertiFlowApp.Data.Configurations;

// EF Core configuration for the Deviation entity
public class DeviationConfiguration
    : IEntityTypeConfiguration<Deviation>
{
    public void Configure(EntityTypeBuilder<Deviation> builder)
    {
        builder.HasKey(deviation => deviation.Id);

        // Description remains PostgreSQL TEXT
        builder.Property(deviation => deviation.Description)
            .IsRequired();

        builder.Property(deviation => deviation.Severity)
            .IsRequired();

        builder.Property(deviation => deviation.Status)
            .HasDefaultValue(DeviationStatus.Open)
            .IsRequired();

        // Prevents deleting the job while it is referenced by a deviation
        builder.HasOne(deviation => deviation.Job)
            .WithMany(job => job.Deviations)
            .HasForeignKey(deviation => deviation.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting the measurement while it is referenced by a deviation
        builder.HasOne(deviation => deviation.Measurement)
            .WithMany()
            .HasForeignKey(deviation => deviation.MeasurementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(deviation => deviation.CreatedAtUtc)
            .IsRequired();

        builder.Property(deviation => deviation.CreatedByUserId)
            .IsRequired();

        // Prevents deleting the user while referenced as the creator
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(deviation => deviation.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting the user while it is referenced as the last editor
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(deviation => deviation.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting the user while it is referenced as the resolver
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(deviation => deviation.ResolvedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}