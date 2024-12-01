using Microsoft.EntityFrameworkCore;
using MyNurseApp.Common.Enums;
using MyNurseApp.Data.Models;
using MyNurseApp.Web.ViewModels.PatientProfile;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Web.ViewModels.HomeVisitation
{
    public class HomeVisitationViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The date and time for the manipulation is required.")]
        [Comment("Date and time for applying the manipulation")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date and time format.")]
        public DateTime DateTimeManipulation { get; set; }

        [Required(ErrorMessage = "The payment method is required.")]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(500, ErrorMessage = "The note cannot exceed 500 characters.")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "The price for the visitation is required.")]
        [Range(20, 500, ErrorMessage = "The price must be between 20 and 500 BGN.")]
        [Comment("Default value is 20 BGN. Extra charge applies if a specific hour is selected.")]
        public decimal PriceForVisitation { get; set; } = 20;

        [Required]
        public bool IsHomeVisitationConfirmed { get; set; } = false;

        [Required]
        public Guid PatientId { get; set; }

        public PatientProfileViewModel? Patient { get; set; }

        public Guid? NurseId { get; set; }

        public bool IsComplete { get; set; }

        [Required]
        public ICollection<MedicalManipulation> MedicalManipulations { get; set; } = new List<MedicalManipulation>();
    }
}
