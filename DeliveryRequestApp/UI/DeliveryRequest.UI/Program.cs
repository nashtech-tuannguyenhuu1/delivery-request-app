using DeliveryRequest.UI.Data;
using DeliveryRequest.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

await DbInitializer.MigrateAndSeedAsync(app.Services);

app.UseApplication();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
