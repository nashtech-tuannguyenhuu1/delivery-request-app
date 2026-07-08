using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Application.Entities;

namespace Report.Infrastructure.Data.Configurations;

public class DailyTrackingConfiguration : IEntityTypeConfiguration<DailyTracking>
{
    public void Configure(EntityTypeBuilder<DailyTracking> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Date);

        builder.Property(x => x.DeliveredCount)
            .HasDefaultValue(0);
        builder.Property(x => x.NewCount)
            .HasDefaultValue(0);
        builder.Property(x => x.AssignedCount)
            .HasDefaultValue(0);
        builder.Property(x => x.ReturnedCount)
            .HasDefaultValue(0);
    }
}
