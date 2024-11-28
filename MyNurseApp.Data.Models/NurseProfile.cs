using Microsoft.EntityFrameworkCore;
using MyNurseApp.Common.Enums;
using MyNurseApp.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Data
{
    public class NurseProfile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(99)]
        [Comment("First name of the nurse")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(99)]
        [Comment("Last name of the nurse")]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(13)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [Comment("Min years of experience for this work")]
        public int YearsOfExperience { get; set; }

        public string? Recommendations { get; set; }

        [Required]
        public NurseEducation Education { get; set; }

        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public NurseStatus IsConfirmed { get; set; } = NurseStatus.Pending;


        public bool IsDeleted { get; set; } = false;
    }
}
