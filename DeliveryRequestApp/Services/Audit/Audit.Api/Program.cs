using Audit.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

app.UseApplication();

app.Run();
