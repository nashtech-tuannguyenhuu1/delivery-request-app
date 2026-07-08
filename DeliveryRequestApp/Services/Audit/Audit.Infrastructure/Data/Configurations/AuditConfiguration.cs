using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuditEntity = Audit.Application.Entities.Audit;

namespace Audit.Infrastructure.Data.Configurations;

public class AuditConfiguration : IEntityTypeConfiguration<AuditEntity>
{
    public void Configure(EntityTypeBuilder<AuditEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TableName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(100);

        builder.Property(x => x.PrimaryKey)
            .HasMaxLength(200);

        builder.HasMany(x => x.AuditProperties)
            .WithOne()
            .HasForeignKey(x => x.AuditId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
