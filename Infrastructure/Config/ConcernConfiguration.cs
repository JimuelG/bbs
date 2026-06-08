using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Config;

public class ConcernConfiguration : IEntityTypeConfiguration<Concern>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Concern> builder)
    {
        builder.Property(c => c.Type).HasConversion<string>();
        builder.Property(c => c.Status).HasConversion<string>();
    }
}
