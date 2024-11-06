using Microsoft.EntityFrameworkCore;
using MyNurseApp.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNurseApp.Data.Models
{
    public class MedicalManipulation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(MedicalManipulationConstants.ManipulationNameMaxLength)]
        [Comment("Name of the manipulation")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(MedicalManipulationConstants.ManipulationMaxDurationTime)]
        [Comment("Duration of single manipualton in minutes")]
        public int Duration { get; set; }

        [MaxLength(MedicalManipulationConstants.ManipulationDescriptionMaxLength)]
        [Comment("Description if the manipulation needed to.")]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(28, 6)")]
        [Precision(18, 2)]
        [Comment("Manipulattion price depents of type of manipulation, location and etc.")]
        public decimal Price { get; set; }
    }
}
