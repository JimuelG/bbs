using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class BarangayCertificateConfiguration : IEntityTypeConfiguration<BarangayCertificate>
{
    public void Configure(EntityTypeBuilder<BarangayCertificate> builder)
    {
        builder.HasIndex(c => c.ReferenceNumber).IsUnique();
        builder.Property(c => c.FullName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Address).HasMaxLength(200);
        builder.Property(c => c.Fee).HasColumnType("decimal(18,2)");
        builder.Property(c => c.CivilStatus).HasMaxLength(20);
        builder.Property(c => c.Purok).HasMaxLength(50);
        builder.Property(c => c.StayDuration).HasMaxLength(100);
        builder.HasOne(c => c.Resident)
                    .WithMany()
                    .HasForeignKey(c => c.ResidentId)
                    .OnDelete(DeleteBehavior.SetNull);
        builder.HasQueryFilter(a => !a.IsDeleted);
        builder.HasQueryFilter(c => c.Resident != null && !c.Resident.IsDeleted);
    }
}
