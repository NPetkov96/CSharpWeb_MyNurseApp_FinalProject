using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data.Models;

namespace MyNurseApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<NurseProfile> NurseProfiles { get; set; }
        public DbSet<HomeVisitation> HomeVisitations { get; set; }
        public DbSet<MedicalManipulation> MedicalManipulations { get; set; }
    }
}
