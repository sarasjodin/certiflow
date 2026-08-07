using CertiFlowApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CertiFlowApp.Data.Configurations;

// EF Core configuration for the Tool entity
public class ToolConfiguration : IEntityTypeConfiguration<Tool>
{
    public void Configure(EntityTypeBuilder<Tool> builder)
    {
        builder.HasKey(tool => tool.Id);

        builder.Property(tool => tool.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(tool => tool.SerialNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(tool => tool.SerialNumber)
            .IsUnique();

        builder.Property(tool => tool.ToolType)
            .HasMaxLength(100)
            .IsRequired();

        // Updated since calibration expires on a calendar date not at a specific time
        // Column becomes nullable since property:
        // public DateOnly? CalibrationValidUntil { get; set; }
        // is set. Therefore .IsRequired(); is not needed.
        builder.Property(tool => tool.CalibrationValidUntil)
            .HasColumnType("date");

        builder.Property(tool => tool.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(tool => tool.CreatedAtUtc)
            .IsRequired();

        builder.Property(tool => tool.CreatedByUserId)
            .IsRequired();


        // Prevents deleting the user while it is referenced as the creator
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(tool => tool.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting the user while it is referenced as the last editor
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(tool => tool.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}