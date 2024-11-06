using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Data.Models
{
    public class HomeVisitation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Comment("Unique Identification Number of the Patient")]
        public string UIN { get; set; } = null!;  // Unique Identification Number

        [Required]
        [Comment("Date and time for applying the manipulation")]
        public DateTime DateTimeManipulation { get; set; }

        [Required]
        public ICollection<MedicalManipulation> MedicalManipulations { get; set; } = new List<MedicalManipulation>();
    }
}
