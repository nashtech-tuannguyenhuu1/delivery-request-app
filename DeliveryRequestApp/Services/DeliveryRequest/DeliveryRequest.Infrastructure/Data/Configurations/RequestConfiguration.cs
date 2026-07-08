using DeliveryRequest.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryRequest.Infrastructure.Data.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasKey(x => x.Id);

        // Key is assigned by the application (Guid.NewGuid()) so it exists before SaveChanges.
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PickupAddress)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.DeliveryAddress)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ReturnedReason)
            .HasMaxLength(1000);

        // Soft-deleted rows are excluded from every query by default.
        builder.HasQueryFilter(x => !x.DeletedById.HasValue);
    }
}
