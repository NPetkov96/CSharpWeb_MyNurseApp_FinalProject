using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data.Models;

namespace MyNurseApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>,Guid>
    {
        public ApplicationDbContext()
        {
            
        }

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<PatientProfile> PatientProfiles { get; set; } = null!;
        public DbSet<NurseProfile> NurseProfiles { get; set; } = null!;
        public DbSet<HomeVisitation> HomeVisitations { get; set; } = null!;
        public DbSet<MedicalManipulation> MedicalManipulations { get; set; } = null!;
    }
}
