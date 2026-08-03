using CertiFlowApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CertiFlowApp.Data.Configurations;

// EF Core configuration for the AuditLog entity
public class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(auditLog => auditLog.Id);

        builder.Property(auditLog => auditLog.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(auditLog => auditLog.EntityId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(auditLog => auditLog.Action)
            .IsRequired();

        builder.Property(auditLog => auditLog.PerformedAtUtc)
            .IsRequired();

        builder.Property(auditLog => auditLog.PerformedByUserId)
            .IsRequired();

        // Prevents cascading deletes when a user is deleted preserving audit logs
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(auditLog => auditLog.PerformedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}