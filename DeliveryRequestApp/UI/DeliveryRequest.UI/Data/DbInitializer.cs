using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRequest.UI.Data;

/// <summary>
/// Applies pending migrations and seeds a default admin account (admin / Admin@123)
/// for local development so login can be tested immediately.
/// </summary>
public static class DbInitializer
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await db.Database.MigrateAsync();

        if (!await db.Users.AnyAsync())
        {
            var hasher = new PasswordHasher<AppUser>();
            var admin = new AppUser
            {
                UserName = "admin",
                DisplayName = "Administrator",
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}
