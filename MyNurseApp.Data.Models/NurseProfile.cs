﻿using Microsoft.EntityFrameworkCore;
using MyNurseApp.Common.Constants;
using MyNurseApp.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNurseApp.Data
{
    public class NurseProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(NurseConstants.NurseMaxNameLength)]
        [Comment("First name of the nurse")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(NurseConstants.NurseMaxNameLength)]
        [Comment("Last name of the nurse")]
        public string LastName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(NurseConstants.NursePhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [Comment("Min years of experience for this work")]
        public int YearsOfExperience { get; set; } 

        public string? Recommendations { get; set; }

        [Required]
        public string Education { get; set; } = null!;

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
