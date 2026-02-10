using System;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Contact).HasMaxLength(20);
        builder.HasIndex(u => u.Contact).IsUnique(true);
        builder.Property(u => u.Address).HasMaxLength(200).IsRequired();
        builder.Property(u => u.IdUrl).IsRequired();
    }
}
