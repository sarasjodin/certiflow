using CertiFlowApp.Models;
using CertiFlowApp.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CertiFlowApp.Data.Configurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasKey(job => job.Id);

            builder.Property(job => job.JobNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(job => job.JobNumber)
                .IsUnique();

            builder.Property(job => job.Title)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(job => job.Status)
                .HasDefaultValue(JobStatus.Draft)
                .IsRequired();

            builder.Property(job => job.CertificateNumber)
                .HasMaxLength(100);

            // Multiple NULL values are allowed by a PostgreSQL unique index
            builder.HasIndex(job => job.CertificateNumber)
                .IsUnique();

            builder.HasOne(job => job.Customer)
                .WithMany(customer => customer.Jobs)
                .HasForeignKey(job => job.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(job => job.CreatedAtUtc)
                .IsRequired();

            builder.Property(job => job.CreatedByUserId)
                .IsRequired();

            // Prevents deleting the user while it is referenced as the creator
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(job => job.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevents deleting the user while it is referenced as the last editor
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(job => job.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevents deleting the user while it is referenced as the approver
            builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(job => job.ApprovedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}