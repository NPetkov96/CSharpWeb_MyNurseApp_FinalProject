using Microsoft.EntityFrameworkCore;
using MyNurseApp.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public decimal PriceForVisitation { get; set; }

        [Required]
        public bool IsHomeVisitationConfirmed { get; set; } = false;

        [Required]
        public Guid PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        [Required]
        public PatientProfile Patient { get; set; } = null!;


        public ICollection<MedicalManipulation> MedicalManipulations { get; set; } = new List<MedicalManipulation>();

        public NurseProfile? Nurse { get; set; }
    }
}
