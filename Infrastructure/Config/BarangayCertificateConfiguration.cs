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
        builder.Property(c => c.Address).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Fee).HasColumnType("decimal(18,2)");
        builder.Property(c => c.Purpose).IsRequired();
        builder.Property(c => c.IssuedBy).IsRequired();
        builder.Property(c => c.BirthDate).IsRequired();
        builder.Property(c => c.CivilStatus).IsRequired().HasMaxLength(20);
        builder.Property(c => c.Purok).IsRequired().HasMaxLength(50);
        builder.Property(c => c.StayDuration).HasMaxLength(100);
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
