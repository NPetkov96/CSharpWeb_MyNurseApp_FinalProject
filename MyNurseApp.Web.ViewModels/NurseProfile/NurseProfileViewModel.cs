using MyNurseApp.Common.Constants;
using MyNurseApp.Common.Enums;
using MyNurseApp.Web.ViewModels.HomeVisitation;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Web.ViewModels.NurseProfile
{
    public class NurseProfileViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [MinLength(NurseConstatns.NameMinLength, ErrorMessage = $"First name must be at least 3 characters long")]
        [MaxLength(NurseConstatns.NameMaxLength, ErrorMessage = $"First name must not exceed 99 characters.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required")]
        [MinLength(NurseConstatns.NameMinLength, ErrorMessage = "Last name must be at least 3 characters long.")]
        [MaxLength(NurseConstatns.NameMaxLength, ErrorMessage = "Last name must not exceed 99 characters.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(NurseConstatns.PhoneNumberMaxLength, MinimumLength = NurseConstatns.PhoneNumberMinLength, ErrorMessage = "Phone number must be between 10 and 13 digits")]
        [RegularExpression(@"^\+?[0-9]{10,13}$", ErrorMessage = "Phone number must contain only digits and may start with +")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Years of experience is required")]
        [Range(NurseConstatns.MinYearsOfExperience, NurseConstatns.MaxYearsOfExperience, ErrorMessage = "Years of experience must be between 0 and 50")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Medical License Number is required.")]
        [StringLength(NurseConstatns.MedicalLicenseNumberMaxLength, MinimumLength = NurseConstatns.MedicalLicenseNumberMinLength, ErrorMessage = "Medical License Number must be between 5 and 50 characters.")]
        [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Medical License Number can only contain uppercase letters, numbers, and dashes.")]
        public string MedicalLicenseNumber { get; set; } = null!;


        [StringLength(NurseConstatns.RecommendationMaxLength, ErrorMessage = "Recommendations cannot exceed 500 characters")]
        public string? Recommendations { get; set; }

        [Required(ErrorMessage = "Education level is required")]
        public NurseEducation Education { get; set; }

        public bool IsRegistrated { get; set; } = false;

        public NurseStatus IsConfirmed { get; set; }

        public Guid UserId { get; set; }

        public ICollection<HomeVisitationViewModel>? HomeVisitations { get; set; } = new List<HomeVisitationViewModel>();
    }
}
