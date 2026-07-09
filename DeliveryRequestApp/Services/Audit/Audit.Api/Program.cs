using Audit.Api.Extensions;
using Audit.Api.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

app.UseApplication();

app.MapAuditEndpoints();

app.Run();
