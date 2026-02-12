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
        builder.Property(c => c.Purpose).IsRequired();
        builder.Property(c => c.IssuedBy).IsRequired();

    }
}
