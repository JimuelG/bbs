using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Config;

public class BarangayOfficialConfiguration : IEntityTypeConfiguration<BarangayOfficial>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BarangayOfficial> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(o => o.LastName).IsRequired().HasMaxLength(50);
        builder.Property(o => o.MiddleName).HasMaxLength(50);
        builder.Property(o => o.Position).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Rank).IsRequired();
        builder.Property(o => o.IsActive).HasDefaultValue(true);
        builder.Property(o => o.OfficeImage).HasMaxLength(500);
        builder.Property(o => o.SignatureImage).HasMaxLength(500);
        builder.HasOne(o => o.Resident).WithMany().HasForeignKey(o => o.ResidentId).OnDelete(DeleteBehavior.SetNull);
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
