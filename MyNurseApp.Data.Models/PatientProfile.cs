using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Data.Models
{
    public class PatientProfile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(99)]
        [Comment("First name of the Patient")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(99)]
        [Comment("Last name of the Patient")]
        public string LastName { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [Comment("Unique Identification Number of the Patient")]
        public string UIN { get; set; } = null!;

        [Required]
        [Comment("Addres for home manipulation of the Patient")]
        public string HomeAddress { get; set; } = null!;

        [Required]
        [StringLength(13)]
        [Comment("Phone number of the Patient")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(99)]
        [Comment("Full name of relative for Emergancy call if it's needed")]
        public string EmergencyContactFullName { get; set; } = null!;

        [Required]
        [StringLength(13)]
        [Comment("Second phone number for Emergancy call if it's needed")]
        public string EmergencyContactPhone { get; set; } = null!;

        public string? Notes { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}
