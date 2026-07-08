using DeliveryRequest.Api.Extensions;
using DeliveryRequest.Api.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

app.UseApplication();

app.MapDeliveryRequestEndpoints();

app.Run();