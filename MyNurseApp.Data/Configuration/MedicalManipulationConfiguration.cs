using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyNurseApp.Data.Models;

namespace MyNurseApp.Data.Configuration
{
    public class MedicalManipulationConfiguration : IEntityTypeConfiguration<MedicalManipulation>
    {
        public void Configure(EntityTypeBuilder<MedicalManipulation> builder)
        {
            builder.Property(m => m.Price)
                   .HasColumnType("decimal(18,2)")
                   .HasPrecision(18,2);
        }
    }
}
