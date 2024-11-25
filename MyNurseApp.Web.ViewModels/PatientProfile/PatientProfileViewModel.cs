using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Web.ViewModels.PatientProfile
{
    public class PatientProfileViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MinLength(3, ErrorMessage = $"First name must be at least 3 characters long")]
        [MaxLength(99, ErrorMessage = $"First name must not exceed 99 characters.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(3, ErrorMessage = "Last name must be at least 3 characters long.")]
        [MaxLength(99, ErrorMessage = "Last name must not exceed 99 characters.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Unique identification number is required.")]
        [MinLength(10, ErrorMessage = "Unique identification number must be exact 10 symbols.")]
        [MaxLength(10, ErrorMessage = "Unique identification number must be exact 10 symbols.")]
        public string UIN { get; set; } = null!;  // Unique Identification Number

        [Required(ErrorMessage = "Home address is required")]
        public string HomeAddress { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [MinLength(10,ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        [MaxLength(13, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Emergency contact's full name is required.")]
        [MinLength(3, ErrorMessage = $"First name must be at least 3 characters long")]
        [MaxLength(99, ErrorMessage = $"First name must not exceed 99 characters.")]
        public string EmergencyContactFullName { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [MinLength(10, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        [MaxLength(13, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        public string EmergencyContactPhone { get; set; } = null!;

        public string? Notes { get; set; }
    }
}
