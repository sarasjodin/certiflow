using System.ComponentModel.DataAnnotations;

namespace CertiFlowApp.Features.Customers
{
    // Sealed because it is a form input model used for UI (not intended to be inherited)
    public sealed class EditCustomerForm
    {
        // Form model used when editing a customer
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(
            150,
            MinimumLength = 2,
            ErrorMessage = "Customer name must contain between 2 and 150 characters")]
        [Display(Name = "Customer name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Organization number is required")]
        [StringLength(
            50,
            ErrorMessage = "Organization number cannot exceed 50 characters")]
        [Display(Name = "Organization number")]
        public string OrganizationNumber { get; set; } = string.Empty;
    }
}
