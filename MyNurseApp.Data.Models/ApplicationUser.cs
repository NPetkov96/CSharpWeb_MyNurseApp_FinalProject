using Microsoft.AspNetCore.Identity;

namespace MyNurseApp.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid();
        }

        public PatientProfile? Patient { get; set; }

        public NurseProfile? Nurse { get; set; }

        public ICollection<HomeVisitation>? PatientHomeVisitations { get; set; } 
            = new List<HomeVisitation>();
    }

}
