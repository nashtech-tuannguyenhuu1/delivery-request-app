using Audit.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audit.Infrastructure.Data.Configurations;

public class AuditDataConfiguration : IEntityTypeConfiguration<AuditData>
{
    public void Configure(EntityTypeBuilder<AuditData> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PropertyName)
            .HasMaxLength(200)
            .IsRequired();
    }
}
