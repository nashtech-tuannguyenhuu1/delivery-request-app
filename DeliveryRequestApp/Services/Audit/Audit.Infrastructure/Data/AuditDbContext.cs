using Audit.Application.Entities;
using Microsoft.EntityFrameworkCore;
using AuditEntity = Audit.Application.Entities.Audit;

namespace Audit.Infrastructure.Data;

public class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuditEntity> Audits => Set<AuditEntity>();

    public DbSet<AuditData> AuditData => Set<AuditData>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditDbContext).Assembly);
    }
}
