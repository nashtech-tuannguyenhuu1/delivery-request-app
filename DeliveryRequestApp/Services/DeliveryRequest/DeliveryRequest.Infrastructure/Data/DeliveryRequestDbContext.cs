using DeliveryRequest.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRequest.Infrastructure.Data;

public class DeliveryRequestDbContext : DbContext
{
    public DeliveryRequestDbContext(DbContextOptions<DeliveryRequestDbContext> options)
        : base(options)
    {
    }

    public DbSet<Request> Requests => Set<Request>();

    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeliveryRequestDbContext).Assembly);
    }
}
