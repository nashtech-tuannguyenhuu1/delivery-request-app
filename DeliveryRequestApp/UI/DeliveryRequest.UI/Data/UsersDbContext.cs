using Microsoft.EntityFrameworkCore;

namespace DeliveryRequest.UI.Data;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(builder =>
        {
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.Property(u => u.UserName).HasMaxLength(100).IsRequired();
            builder.Property(u => u.DisplayName).HasMaxLength(200).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired();
        });
    }
}
