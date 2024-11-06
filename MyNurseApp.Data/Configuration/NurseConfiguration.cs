using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyNurseApp.Data.Configuration
{
    public class NurseConfiguration : IEntityTypeConfiguration<NurseProfile>
    {
        public void Configure(EntityTypeBuilder<NurseProfile> builder)
        {
            builder.Property(p => p.IsDeleted)
                   .HasDefaultValue(false);

            builder.HasOne(a => a.User)
                   .WithOne(p => p.Nurse)
                   .HasForeignKey<NurseProfile>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(n => n.UserId)
                   .IsUnique();
        }
    }
}
