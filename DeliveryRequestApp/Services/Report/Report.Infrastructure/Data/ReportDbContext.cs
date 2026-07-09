using Microsoft.EntityFrameworkCore;
using Report.Application.Entities;

namespace Report.Infrastructure.Data;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<DailyTracking> DailyTrackings { get; set; }

    public virtual DbSet<RequestStatusTracking> RequestStatusTrackings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportDbContext).Assembly);
    }
}
