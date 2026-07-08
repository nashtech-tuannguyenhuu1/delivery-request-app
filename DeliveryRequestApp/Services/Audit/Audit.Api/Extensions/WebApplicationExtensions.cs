using Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Audit.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApplication(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.ApplyMigrations();

        app.UseHttpsRedirection();

        return app;
    }

    private static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
        dbContext.Database.Migrate();

        return app;
    }
}
