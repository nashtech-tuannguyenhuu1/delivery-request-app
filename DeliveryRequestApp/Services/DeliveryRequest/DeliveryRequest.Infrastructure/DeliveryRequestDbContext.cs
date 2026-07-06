using DeliveryRequest.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRequest.Infrastructure;

public class DeliveryRequestDbContext : DbContext
{
    public DeliveryRequestDbContext(DbContextOptions<DeliveryRequestDbContext> options)
        : base(options)
    {
    }

    public DbSet<Request> Requests => Set<Request>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeliveryRequestDbContext).Assembly);
    }
}
