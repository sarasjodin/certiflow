using CertiFlowApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CertiFlowApp.Data.Configurations;

// EF Core configuration for the Customer entity.
public class CustomerConfiguration
    : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(customer => customer.OrganizationNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(customer => customer.OrganizationNumber)
            .IsUnique();

        builder.Property(customer => customer.CreatedAtUtc)
            .IsRequired();

        builder.Property(customer => customer.CreatedByUserId)
            .IsRequired();

        // Prevents deleting the user while referenced as the creator
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(customer => customer.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents deleting the user while referenced as the updater
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(customer => customer.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}