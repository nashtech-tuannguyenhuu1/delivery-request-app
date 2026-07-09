using Report.Api.Extensions;
using Report.Api.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

app.UseApplication();

app.MapReportEndpoints();

app.Run();
