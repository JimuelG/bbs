using System;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class AnnouncementCofiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.Property(a => a.Title).HasMaxLength(150).IsRequired();
        builder.Property(a => a.Message).IsRequired();
    }
}
