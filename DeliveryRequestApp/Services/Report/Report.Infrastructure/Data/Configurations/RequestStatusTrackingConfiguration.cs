using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Application.Entities;

namespace Report.Infrastructure.Data.Configurations;

public class RequestStatusTrackingConfiguration : IEntityTypeConfiguration<RequestStatusTracking>
{
    public void Configure(EntityTypeBuilder<RequestStatusTracking> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.RequestId)
            .IsUnique();
    }
}
