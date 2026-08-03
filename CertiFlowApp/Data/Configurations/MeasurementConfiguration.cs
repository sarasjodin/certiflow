using CertiFlowApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CertiFlowApp.Data.Configurations;

// EF Core configuration for the Measurement entity.
public class MeasurementConfiguration
    : IEntityTypeConfiguration<Measurement>
{
    public void Configure(EntityTypeBuilder<Measurement> builder)
    {
        builder.HasKey(measurement => measurement.Id);

        builder.Property(measurement => measurement.Value)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(measurement => measurement.Unit)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(measurement => measurement.Status)
            .IsRequired();

        builder.Property(measurement => measurement.MeasuredAtUtc)
            .IsRequired();

        builder.Property(measurement => measurement.PerformedByUserId)
            .IsRequired();

        builder.Property(measurement => measurement.CreatedAtUtc)
            .IsRequired();

        builder.Property(measurement => measurement.CreatedByUserId)
            .IsRequired();

        // Prevents deleting a job while it is referenced by measurements
        builder.HasOne(measurement => measurement.Job)
            .WithMany(job => job.Measurements)
            .HasForeignKey(measurement => measurement.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting a tool while it is referenced by measurements
        builder.HasOne(measurement => measurement.Tool)
            .WithMany(tool => tool.Measurements)
            .HasForeignKey(measurement => measurement.ToolId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting the user while referenced as the operator
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(measurement => measurement.PerformedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting the user while referenced as the verifier
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(measurement => measurement.VerifiedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting the user while referenced as the creator
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(measurement => measurement.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting the user while referenced as the last editor
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(measurement => measurement.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}