using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyNurseApp.Data.Configuration
{
    public class NurseConfiguration : IEntityTypeConfiguration<NurseProfile>
    {
        public void Configure(EntityTypeBuilder<NurseProfile> builder)
        {
            builder.Property(n => n.IsDeleted).HasDefaultValue(false);

            builder.HasOne(n => n.User)
                   .WithOne(u => u.Nurse)
                   .HasForeignKey<NurseProfile>(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(n => n.UserId).IsUnique();
        }
    }
}
