using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyNurseApp.Data.Models;

namespace MyNurseApp.Data.Configuration
{
    public class PatientConfiguration : IEntityTypeConfiguration<PatientProfile>
    {
        public void Configure(EntityTypeBuilder<PatientProfile> builder)
        {
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.HasOne(p => p.User)
                   .WithOne(u => u.Patient)
                   .HasForeignKey<PatientProfile>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.UserId).IsUnique();
        }
    }
}
