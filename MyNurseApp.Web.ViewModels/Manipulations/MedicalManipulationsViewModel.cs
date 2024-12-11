using MyNurseApp.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Web.ViewModels.Manipulations
{
    public class MedicalManipulationsViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(MedicalManipulationsConstants.NameMinLength, ErrorMessage = $"The name must be at least 3 characters long")]
        [MaxLength(MedicalManipulationsConstants.NameMaxLength, ErrorMessage = $"The ame cannot exceed 100 characters.")]
        [Display(Name = "Manipulation Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "The Duration is required.")]
        [Range(MedicalManipulationsConstants.DurationMinLength, MedicalManipulationsConstants.DurationMaxLength, ErrorMessage = "Duration must be between 20 and 120 minutes.")]
        [Display(Name = "Duration (in minutes)")]
        public int Duration { get; set; }

        [StringLength(MedicalManipulationsConstants.DescriptionMaxLength, ErrorMessage = "The Description cannot exceed 200 characters.")]
        [Display(Name = "Description (optional)")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The Price is required.")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price in BGN")]
        public decimal Price { get; set; }

        public IEnumerable<MedicalManipulationsViewModel> ChoosenManilupation { get; set; } = new List<MedicalManipulationsViewModel>();
    }
}
