using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNurseApp.Data.Models
{
    public class MedicalManipulation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Comment("Name of the manipulation")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        [Comment("Duration of single manipualton in minutes")]
        public int Duration { get; set; }

        [MaxLength(200)]
        [Comment("Description if the manipulation needed to.")]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(28, 6)")]
        [Precision(18, 2)]
        [Comment("Manipulattion price depents of type of manipulation, location and etc.")]
        public decimal Price { get; set; }
    }
}
