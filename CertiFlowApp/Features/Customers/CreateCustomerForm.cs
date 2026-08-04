using System.ComponentModel.DataAnnotations;

namespace CertiFlowApp.Features.Customers
{
    // Separate form model
    public class CreateCustomerForm
    {
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
