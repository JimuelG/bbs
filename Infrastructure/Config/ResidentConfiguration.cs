using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class ResidentConfiguration : IEntityTypeConfiguration<Resident>
{
    public void Configure(EntityTypeBuilder<Resident> builder)
    {
        builder.Property(r => r.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.LastName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Purok).IsRequired().HasMaxLength(50);

        builder.Property(r => r.MonthlyIncome)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);
        
        builder.HasOne(r => r.AppUser)
            .WithOne(u => u.Resident)
            .HasForeignKey<Resident>(r => r.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
