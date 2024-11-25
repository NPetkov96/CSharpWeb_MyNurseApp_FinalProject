using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Data.Models
{
    public class HomeVisitation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Comment("Date and time for applying the manipulation")]
        public DateTime DateTimeManipulation { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public string? Note { get; set; }

        [Required]
        public decimal PriceForVisitation { get; set; } //Default value is 20 BGN if Patient choose a hour for visitation i will have extra charge.

        [Required]
        public bool IsHomeVisitationConfirmed { get; set; } = false;
        
        [Required]
        public ApplicationUser UserId { get; set; } = null!;

        [Required]
        public ICollection<MedicalManipulation> MedicalManipulations { get; set; } = new List<MedicalManipulation>();
    }
}
