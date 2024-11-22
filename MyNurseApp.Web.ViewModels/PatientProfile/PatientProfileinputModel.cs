using MyNurseApp.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Web.ViewModels.PatientProfile
{
    public class PatientProfileinputModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(PatientConstants.PatientMinNameLength, ErrorMessage = $"First name must be at least 3 characters long")]
        [MaxLength(PatientConstants.PatientMaxNameLength, ErrorMessage = $"First name must not exceed 99 characters.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(PatientConstants.PatientMinNameLength, ErrorMessage = "Last name must be at least 3 characters long.")]
        [MaxLength(PatientConstants.PatientMaxNameLength, ErrorMessage = "Last name must not exceed 99 characters.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Home address is required")]
        public string HomeAddress { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [MinLength(PatientConstants.PatientPhoneNumberMinLength,ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        [MaxLength(PatientConstants.PatientPhoneNumberMaxLength, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Emergency contact's full name is required.")]
        [MinLength(PatientConstants.PatientMinNameLength, ErrorMessage = $"First name must be at least 3 characters long")]
        [MaxLength(PatientConstants.PatientMaxNameLength, ErrorMessage = $"First name must not exceed 99 characters.")]
        public string EmergencyContactFullName { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [MinLength(PatientConstants.PatientPhoneNumberMinLength, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        [MaxLength(PatientConstants.PatientPhoneNumberMaxLength, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        public string EmergencyContactPhone { get; set; } = null!;

        public string? Notes { get; set; }
    }
}
