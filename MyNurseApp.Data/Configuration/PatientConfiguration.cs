using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyNurseApp.Data.Configuration
{
    public class PatientConfiguration : IEntityTypeConfiguration<PatientProfile>
    {
        public void Configure(EntityTypeBuilder<PatientProfile> builder)
        {
            builder.Property(p => p.IsDeleted)
                   .HasDefaultValue(false);

            builder.HasOne(a => a.User)
                   .WithOne(p => p.Patient)
                   .HasForeignKey<PatientProfile>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(n => n.UserId)
                   .IsUnique();
        }
    }
}
