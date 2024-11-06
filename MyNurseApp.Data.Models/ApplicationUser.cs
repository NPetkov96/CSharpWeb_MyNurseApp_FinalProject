using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNurseApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public PatientProfile? Patient { get; set; }

        public int? NurseId { get; set; }
        [ForeignKey(nameof(NurseId))]
        public NurseProfile? Nurse { get; set; }
    }

}
