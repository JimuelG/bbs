using System;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasOne(u => u.Resident)
            .WithOne(r => r.AppUser)
            .HasForeignKey<AppUser>(u => u.ResidentId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Property(u => u.IdUrl).HasMaxLength(500);
        
    }
}
