using MyNurseApp.Common.Constants;
using MyNurseApp.Common.DatetimeValidations.PatientProfile;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Web.ViewModels.PatientProfile
{
    public class PatientProfileViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MinLength(PatientConstants.PatientNameMinLength, ErrorMessage = $"First name must be at least 3 characters long")]
        [MaxLength(PatientConstants.PatientNameMaxLength, ErrorMessage = $"First name must not exceed 99 characters.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(PatientConstants.NameMinLength, ErrorMessage = "Last name must be at least 3 characters long.")]
        [MaxLength(PatientConstants.NameMaxLength, ErrorMessage = "Last name must not exceed 99 characters.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DateOfBirthValidator), nameof(DateOfBirthValidator.ValidateDateOfBirth))]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Unique identification number is required.")]
        [MinLength(PatientConstants.UinLength, ErrorMessage = "Unique identification number must be exact 10 symbols.")]
        [MaxLength(PatientConstants.UinLength, ErrorMessage = "Unique identification number must be exact 10 symbols.")]
        public string UIN { get; set; } = null!;

        [Required(ErrorMessage = "Home address is required")]
        public string HomeAddress { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [MinLength(PatientConstants.PhoneNumberMinLength, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        [MaxLength(PatientConstants.PhoneNumberMaxLength, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Emergency contact's full name is required.")]
        [MinLength(PatientConstants.NameMinLength, ErrorMessage = $"First name must be at least 3 characters long")]
        [MaxLength(PatientConstants.NameMaxLength, ErrorMessage = $"First name must not exceed 99 characters.")]
        public string EmergencyContactFullName { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [MinLength(PatientConstants.PhoneNumberMinLength, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        [MaxLength(PatientConstants.PhoneNumberMaxLength, ErrorMessage = "Phone number must be betweeen 10 and 13 characters.")]
        public string EmergencyContactPhone { get; set; } = null!;

        public string? Notes { get; set; }
    }
}
